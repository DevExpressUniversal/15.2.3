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
	public abstract class TransposeHistoryItem<T> : DashboardItemHistoryItem<T> where T : DataDashboardItem {
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemModifyBindings; } }
		protected abstract DimensionCollection Dimensions1 { get; }
		protected abstract DimensionCollection Dimensions2 { get; }
		protected TransposeHistoryItem(T dashboardItem)
			: base(dashboardItem) {
		}
		protected override void PerformUndo() {
			Transpose();
		}
		protected override void PerformRedo() {
			Transpose();
		}
		void Transpose() {
			DashboardItem.Dashboard.BeginUpdate();
			try {
				Dimension[] dimensions1 = Dimensions1.ToArray();
				Dimensions1.Clear();
				Dimension[] dimensions2 = Dimensions2.ToArray();
				Dimensions2.Clear();
				Dimensions2.AddRange(dimensions1);
				Dimensions1.AddRange(dimensions2);
			}
			finally {
				DashboardItem.Dashboard.EndUpdate();
			}
		}
	}
	public class TransposeChartHistoryItem : TransposeHistoryItem<ChartDashboardItem> {
		public TransposeChartHistoryItem(ChartDashboardItem dashboardItem)
			: base(dashboardItem) {
		}
		protected override DimensionCollection Dimensions1 { get { return (DashboardItem as ChartDashboardItem).Arguments; } }
		protected override DimensionCollection Dimensions2 { get { return (DashboardItem as ChartDashboardItem).SeriesDimensions; } }
	}
	public class TransposePieHistoryItem : TransposeHistoryItem<PieDashboardItem> {
		public TransposePieHistoryItem(PieDashboardItem dashboardItem)
			: base(dashboardItem) {
		}
		protected override DimensionCollection Dimensions1 { get { return (DashboardItem as PieDashboardItem).Arguments; } }
		protected override DimensionCollection Dimensions2 { get { return (DashboardItem as PieDashboardItem).SeriesDimensions; } }
	}
	public class TransposePivotHistoryItem : TransposeHistoryItem<PivotDashboardItem> {
		public TransposePivotHistoryItem(PivotDashboardItem dashboardItem)
			: base(dashboardItem) {
		}
		protected override DimensionCollection Dimensions1 { get { return (DashboardItem as PivotDashboardItem).Columns; } }
		protected override DimensionCollection Dimensions2 { get { return (DashboardItem as PivotDashboardItem).Rows; } }
	}
	public class TransposeScatterChartHistoryItem : DashboardItemHistoryItem<ScatterChartDashboardItem> {
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemModifyBindings; } }
		ScatterChartDashboardItem Scatter { get { return (ScatterChartDashboardItem)DashboardItem; } }
		public TransposeScatterChartHistoryItem(ScatterChartDashboardItem dashboardItem)
			: base(dashboardItem) {
		}
		protected override void PerformUndo() {
			Transpose();
		}
		protected override void PerformRedo() {
			Transpose();
		}
		void Transpose() {
			DashboardItem.Dashboard.BeginUpdate();
			try {
				Measure measureX = Scatter.AxisXMeasure.Clone();
				Measure measureY = Scatter.AxisYMeasure.Clone();
				Scatter.AxisXMeasure = measureY;
				Scatter.AxisYMeasure = measureX;
			}
			finally {
				DashboardItem.Dashboard.EndUpdate();
			}
		}
	}
}
