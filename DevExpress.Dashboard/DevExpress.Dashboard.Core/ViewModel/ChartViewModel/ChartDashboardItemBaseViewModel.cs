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
using System.Linq;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.ViewModel {
	public class SelectionViewModel {
		public string[] LegendIds { get; set; }
		public string[] ArgumentIds { get; set; }
		public string[] SeriesIds { get; set; }
		public string[] PointIds { get; set; }
	}
	[Flags]
	public enum ChartSelectionModeViewModel {
		Argument = 1,
		Series = 2,
		Points = Argument | Series
	}
	public abstract class ChartDashboardItemBaseViewModel : DataDashboardItemViewModel {
		ChartSelectionModeViewModel selectionMode;
		bool selectionEnabled;
		string[] actualArgumentDataMembers;
		string[] actualSeriesDataMembers;
		public string ArgumentColorDimension { get; set; }
		public string SeriesColorDimension { get; set; }
		public string[] ColorPathMembers { get; set; }
		public string SummarySeriesMember { get; set; }
		public ChartArgumentViewModel Argument { get; set; }
		public SelectionViewModel SelectionModel { get; set; }
		public ChartSelectionModeViewModel SelectionMode {
			get { return selectionMode; }
			set { selectionMode = value; }
		}
		public bool SelectionEnabled {
			get { return selectionEnabled; }
			set { selectionEnabled = value; }
		}
		public string[] ActualArgumentDataMembers {
			get { return actualArgumentDataMembers; }
			set { actualArgumentDataMembers = value; }
		}
		public string[] ActualSeriesDataMembers {
			get { return actualSeriesDataMembers; }
			set { actualSeriesDataMembers = value; }
		}
		protected ChartDashboardItemBaseViewModel()
			: base() {
				Argument = new ChartArgumentViewModel();
		}
		protected ChartDashboardItemBaseViewModel(ChartDashboardItemBase dashboardItem)
			: base(dashboardItem) {
			Argument = new ChartArgumentViewModel(dashboardItem, dashboardItem.Arguments.Count > 0);
			Dimension argumentDimension = dashboardItem.IsDrillDownEnabledOnArguments ? dashboardItem.CurrentDrillDownDimension : dashboardItem.Arguments.LastOrDefault();
			Argument.SummaryArgumentMember = argumentDimension != null ? argumentDimension.ActualId : null;
			Dimension seriesDimension = dashboardItem.IsDrillDownEnabledOnSeries ? dashboardItem.CurrentDrillDownDimension : dashboardItem.SeriesDimensions.LastOrDefault();
			SummarySeriesMember = seriesDimension != null ? seriesDimension.ActualId : null;
			selectionEnabled = dashboardItem.IsSelectionEnabled;
			actualArgumentDataMembers = dashboardItem.GetArgumentDimensionsUniqueNames();
			actualSeriesDataMembers = dashboardItem.GetSeriesDimensionUniqueNames();
			ArgumentColorDimension = dashboardItem.GetColorDimensionsByAxis(DashboardDataAxisNames.ChartArgumentAxis).Keys.LastOrDefault();
			SeriesColorDimension = dashboardItem.GetColorDimensionsByAxis(DashboardDataAxisNames.ChartSeriesAxis).Keys.LastOrDefault();
			ColorPathMembers = dashboardItem.GetColorPath();
			TargetDimensions targetDimensions = dashboardItem.InteractivityOptions.TargetDimensions;
			selectionMode = (ChartSelectionModeViewModel)targetDimensions;
			SelectionModel = new SelectionViewModel();
			if(targetDimensions.HasFlag(TargetDimensions.Arguments))
				SelectionModel.ArgumentIds = actualArgumentDataMembers;
			if(targetDimensions.HasFlag(TargetDimensions.Series))
				SelectionModel.SeriesIds = actualSeriesDataMembers;
			if(targetDimensions.HasFlag(TargetDimensions.Points)) {
				SelectionModel.PointIds = actualArgumentDataMembers.Concat(actualSeriesDataMembers).ToArray();
				SelectionModel.LegendIds = ColorPathMembers;
			}
		}
		protected ChartDashboardItemBaseViewModel(ScatterChartDashboardItem dashboardItem)
			: base(dashboardItem) {
			Argument = new ChartArgumentViewModel(dashboardItem);
			Dimension argumentDimension = dashboardItem.IsDrillDownEnabled ? dashboardItem.CurrentDrillDownDimension : dashboardItem.Arguments.LastOrDefault();
			Argument.SummaryArgumentMember = argumentDimension != null ? argumentDimension.ActualId : null;
			selectionMode = ChartSelectionModeViewModel.Argument;
			selectionEnabled = dashboardItem.IsSelectionEnabled;
			actualArgumentDataMembers = dashboardItem.GetDimensionActualIds();
			actualSeriesDataMembers = new string[0];
			ArgumentColorDimension = dashboardItem.GetColorDimensionsByAxis(DashboardDataAxisNames.ChartArgumentAxis).Keys.LastOrDefault();
			ColorPathMembers = dashboardItem.GetColorPath();
		}
	}
}
