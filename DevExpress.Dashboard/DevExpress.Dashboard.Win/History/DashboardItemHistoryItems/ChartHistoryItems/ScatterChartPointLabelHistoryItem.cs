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
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public class ScatterChartPointLabelHistoryItem : DashboardItemHistoryItem<ScatterChartDashboardItem> {
		readonly ScatterChartPointLabelSettings initialSettings;
		ScatterChartPointLabelSettings finalSettings;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemScatterChartPointLabelOptions; } }
		protected ScatterPointLabelOptions LabelOptions { get { return DashboardItem.PointLabelOptions; } }
		public ScatterChartPointLabelSettings Settings { get { return finalSettings; } }
		public bool PositionEnabled { get { return Scatter.Weight != null; } }
		ScatterChartDashboardItem Scatter { get { return (ScatterChartDashboardItem)DashboardItem; } }
		public ScatterChartPointLabelHistoryItem(ScatterChartDashboardItem dashboardItem)
			: base(dashboardItem) {
			initialSettings = GetSettings(LabelOptions);
			finalSettings = GetSettings(LabelOptions);
		}
		public void UpdateSettings(ScatterChartPointLabelSettings settings) {
			finalSettings = settings;
		}
		protected override void PerformUndo() {
			ApplySettings(initialSettings);
		}
		protected override void PerformRedo() {
			ApplySettings(finalSettings);
		}
		void ApplySettings(ScatterChartPointLabelSettings settings) {
			LabelOptions.Content = settings.Content; 
			LabelOptions.Orientation = settings.Orientation;
			LabelOptions.OverlappingMode = settings.OverlappingMode;
			LabelOptions.ShowPointLabels = settings.ShowPointLabels;
			LabelOptions.Position = settings.Position;
		}
		ScatterChartPointLabelSettings GetSettings(ScatterPointLabelOptions labelOptions) {
			return new ScatterChartPointLabelSettings {
				Content = labelOptions.Content,
				Orientation = labelOptions.Orientation,
				OverlappingMode = labelOptions.OverlappingMode,
				ShowPointLabels = labelOptions.ShowPointLabels,
				Position = labelOptions.Position
			};
		}
	}
	public class ScatterChartPointLabelSettings {
		public ScatterPointLabelContentType Content { get; set; }
		public PointLabelOrientation Orientation { get; set; }
		public PointLabelOverlappingMode OverlappingMode { get; set; }
		public PointLabelPosition Position { get; set; }
		public bool ShowPointLabels { get; set; }
	}
}
