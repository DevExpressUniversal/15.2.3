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
namespace DevExpress.DashboardWin.Native {
	public abstract class MapLegendPositionHistoryItemBase : DashboardItemHistoryItem<MapDashboardItem> {
		readonly MapLegendBase legend;
		readonly MapLegendPosition initialPosition;
		readonly MapLegendPosition currentPosition;
		protected MapLegendBase Legend { get { return legend; } }
		protected MapLegendPositionHistoryItemBase(MapDashboardItem dashboardItem, MapLegendBase legend, MapLegendPosition currentPosition)
			: base(dashboardItem) {
			this.legend = legend;
			initialPosition = legend.Position;
			this.currentPosition = currentPosition;
		}
		protected virtual void PerformUndoInternal() {
			legend.Position = initialPosition;
		}
		protected virtual void PerformRedoInternal() {
			legend.Position = currentPosition;
		}
		protected override void PerformUndo() {
			legend.BeginUpdate();
			try {
				PerformUndoInternal();
			}
			finally {
				legend.EndUpdate();
			}
		}
		protected override void PerformRedo() {
			legend.BeginUpdate();
			try {
				PerformRedoInternal();
			}
			finally {
				legend.EndUpdate();
			}
		}
	}
	public class MapLegendPositionHistoryItem : MapLegendPositionHistoryItemBase {
		readonly MapLegendOrientation initialOrientation;
		readonly MapLegendOrientation currentOrientation;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemChartLegendPosition; } }
		public MapLegendPositionHistoryItem(MapDashboardItem dashboardItem, MapLegendPosition currentLegendPosition, MapLegendOrientation currentLegendOrientation)
			: base(dashboardItem, dashboardItem.Legend, currentLegendPosition) {
			initialOrientation = dashboardItem.Legend.Orientation;
			this.currentOrientation = currentLegendOrientation;
		}
		protected override void PerformUndoInternal() {
			base.PerformUndoInternal();
			((MapLegend)Legend).Orientation = initialOrientation;
		}
		protected override void PerformRedoInternal() {
			base.PerformRedoInternal();
			((MapLegend)Legend).Orientation = currentOrientation;
		}
	}
	public class WeightedLegendPositionHistoryItem : MapLegendPositionHistoryItemBase {
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemChartLegendPosition; } }
		public WeightedLegendPositionHistoryItem(MapDashboardItem dashboardItem, MapLegendPosition currentLegendPosition)
			: base(dashboardItem, dashboardItem.WeightedLegend, currentLegendPosition) {
		}
	}
}
