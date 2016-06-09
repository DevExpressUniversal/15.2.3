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
	public abstract class ChartYAxisSettingsHistoryItemBase<T> : DashboardItemHistoryItem<T>, IChartYAxisSettingsHistoryItem
		where T : IChartDashboardItem {
		readonly List<ChartAxisYSettings> initialSettings = new List<ChartAxisYSettings>();
		readonly List<ChartAxisYSettings> finalSettings = new List<ChartAxisYSettings>();
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemChartYAxisSettings; } }
		public List<ChartAxisYSettings> Settings { get { return finalSettings; } }
		public abstract bool PanesEnabled { get; }
		public abstract bool AlwaysShowZeroLevelEnabled { get; }
		public abstract string AxisSettingsName { get; }
		protected List<ChartAxisYSettings> InitialSettings { get { return initialSettings; } }
		protected List<ChartAxisYSettings> FinalSettings { get { return finalSettings; } }
		protected ChartYAxisSettingsHistoryItemBase(T dashboardItem)
			: base(dashboardItem) {
		}
		protected override void PerformUndo() {
			foreach(ChartAxisYSettings settings in initialSettings)
				settings.Apply();
		}
		protected override void PerformRedo() {
			foreach(ChartAxisYSettings settings in finalSettings)
				settings.Apply();
		}
		public void UpdateSettings(ChartAxisY axis) {
			ChartAxisYSettings oldSettings = finalSettings.Find(x => x.ChartAxisY == axis);
			finalSettings.Remove(oldSettings);
			finalSettings.Add(oldSettings.GenerateAxisYSettings(axis));
		}
	}
	public class ChartYAxisSettingsHistoryItem : ChartYAxisSettingsHistoryItemBase<ChartDashboardItem> {
		public override bool PanesEnabled { get { return true; } }
		public override bool AlwaysShowZeroLevelEnabled { get { return true; } }
		public override string AxisSettingsName { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandChartYAxisSettingsCaption); } }
		public ChartYAxisSettingsHistoryItem(ChartDashboardItem dashboardItem)
			: base(dashboardItem) {
			foreach(ChartPane pane in dashboardItem.Panes) {
				bool hasSecondaryAxis = pane.Series.FindFirst(series => series.PlotOnSecondaryAxis) != null;
				InitialSettings.Add(new ChartAxisYSettings(pane.Name, pane.PrimaryAxisY, hasSecondaryAxis));
				FinalSettings.Add(new ChartAxisYSettings(pane.Name, pane.PrimaryAxisY, hasSecondaryAxis));
				if(hasSecondaryAxis) {
					InitialSettings.Add(new ChartSecondaryAxisYSettings(pane.Name, pane.SecondaryAxisY));
					FinalSettings.Add(new ChartSecondaryAxisYSettings(pane.Name, pane.SecondaryAxisY));
				}
			}
		}
	}
	public abstract class ScatterChartAxisSettingsHistoryItem : ChartYAxisSettingsHistoryItemBase<ScatterChartDashboardItem> {
		public override bool PanesEnabled { get { return false; } }
		protected ScatterChartAxisSettingsHistoryItem(ScatterChartDashboardItem dashboardItem)
			: base(dashboardItem) {
			InitialSettings.Add(new ChartAxisYSettings(null, GetAxis(dashboardItem), false));
			FinalSettings.Add(new ChartAxisYSettings(null, GetAxis(dashboardItem), false));
		}
		protected abstract ChartAxisY GetAxis(ScatterChartDashboardItem dashboardItem);
	}
	public class ScatterChartXAxisSettingsHistoryItem : ScatterChartAxisSettingsHistoryItem {
		public override bool AlwaysShowZeroLevelEnabled { get { return false; } }
		public override string AxisSettingsName { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandScatterChartXAxisSettingsCaption); } }
		public ScatterChartXAxisSettingsHistoryItem(ScatterChartDashboardItem dashboardItem)
			: base(dashboardItem) {
		}
		protected override ChartAxisY GetAxis(ScatterChartDashboardItem dashboardItem) {
			return dashboardItem.AxisX;
		}
	}
	public class ScatterChartYAxisSettingsHistoryItem : ScatterChartAxisSettingsHistoryItem {
		public override bool AlwaysShowZeroLevelEnabled { get { return true; } }
		public override string AxisSettingsName { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandScatterChartYAxisSettingsCaption); } }
		public ScatterChartYAxisSettingsHistoryItem(ScatterChartDashboardItem dashboardItem)
			: base(dashboardItem) {
		}
		protected override ChartAxisY GetAxis(ScatterChartDashboardItem dashboardItem) {
			return dashboardItem.AxisY;
		}
	}
	public interface IChartYAxisSettingsHistoryItem {
		List<ChartAxisYSettings> Settings { get; }
		bool PanesEnabled { get; }
		bool AlwaysShowZeroLevelEnabled { get; }
		void UpdateSettings(ChartAxisY axis);
		string AxisSettingsName { get; }
	}
}
