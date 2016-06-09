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
using System.ComponentModel;
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class AxisXSettingsControl : DashboardUserControl {
		readonly Locker locker = new Locker();
		ChartXAxisSettingsHistoryItem historyItem;
		ChartAxisX currentChartAxisX;
		public AxisXSettingsControl() {
			InitializeComponent();
		}
		public void Initialize(ChartXAxisSettingsHistoryItem historyItem) {
			this.historyItem = historyItem;
			currentChartAxisX = historyItem.Settings.ChartAxisX;
			ceVisible.Checked = currentChartAxisX.Visible;
			ceTitleVisible.Checked = currentChartAxisX.TitleVisible;
			ceReverse.Checked = currentChartAxisX.Reverse;
			if (string.IsNullOrEmpty(currentChartAxisX.Title))
				rbDefaultTitleText.Checked = true;
			else
				rbCustomTitleText.Checked = true;
			cbZoomEnabled.Checked = currentChartAxisX.EnableZooming;
			cbLimitVisiblePoints.Checked = currentChartAxisX.LimitVisiblePoints;
			seVisiblePointsCount.Value = currentChartAxisX.VisiblePointsCount;
			UpdateControlsState();
		}
		void UpdateHistoryItem() {
			historyItem.UpdateSettings(currentChartAxisX);
		}
		void UpdateTitleTextControlState() {
			teTitleText.Enabled = currentChartAxisX.TitleVisible && rbCustomTitleText.Checked;
			locker.Lock();
			try {
				teTitleText.Text = currentChartAxisX.DisplayTitle;
			}
			finally {
				locker.Unlock();
			}
		}
		void UpdateTitleControlsState() {
			bool visible = currentChartAxisX.TitleVisible;
			panelTitleText.Enabled = visible;
			UpdateTitleTextControlState();
		}
		void UpdateControlsState() {
			bool visible = currentChartAxisX.Visible;
			ceTitleVisible.Enabled = visible;
			panelTitle.Enabled = visible;
			cbLimitVisiblePoints.Enabled = !historyItem.Settings.IsContinuousArgumentScale;
			seVisiblePointsCount.Enabled = cbLimitVisiblePoints.Enabled && currentChartAxisX.LimitVisiblePoints;
			UpdateTitleControlsState();
		}
		void OnVisibleCheckedChanged(object sender, EventArgs e) {
			currentChartAxisX.Visible = ceVisible.Checked;
			UpdateControlsState();
			UpdateHistoryItem();
		}
		void OnTitleVisibleCheckedChanged(object sender, EventArgs e) {
			currentChartAxisX.TitleVisible = ceTitleVisible.Checked;
			UpdateTitleControlsState();
			UpdateHistoryItem();
		}
		void OnTitleDefaultTextCheckedChanged(object sender, EventArgs e) {
			currentChartAxisX.Title = rbDefaultTitleText.Checked ? String.Empty : teTitleText.Text;
			rbCustomTitleText.Checked = !rbDefaultTitleText.Checked;
			UpdateTitleTextControlState();
			UpdateHistoryItem();
		}
		void OnTitleCustomTextCheckedChanged(object sender, EventArgs e) {
			rbDefaultTitleText.Checked = !rbCustomTitleText.Checked;
		}
		void OnTitleTextEditValueChanged(object sender, EventArgs e) {
			if (!locker.IsLocked) {
				currentChartAxisX.Title = teTitleText.Text;
				UpdateHistoryItem();
			}
		}
		void OnReverseCheckedChanged(object sender, EventArgs e) {
			currentChartAxisX.Reverse = ceReverse.Checked;
			UpdateHistoryItem();
		}
		void onLimitVisiblePointsCheckedChanged(object sender, EventArgs e) {
			currentChartAxisX.LimitVisiblePoints = cbLimitVisiblePoints.Checked;
			UpdateControlsState();
			UpdateHistoryItem();
		}
		void onZoomEnabledCheckedChanged(object sender, EventArgs e) {
			currentChartAxisX.EnableZooming = cbZoomEnabled.Checked;
			UpdateControlsState();
			UpdateHistoryItem();
		}
		void onVisiblePointsCountEditValueChanged(object sender, EventArgs e) {
			currentChartAxisX.VisiblePointsCount = Convert.ToInt32(seVisiblePointsCount.Value);
			UpdateControlsState();
			UpdateHistoryItem();
		}
	}
}
