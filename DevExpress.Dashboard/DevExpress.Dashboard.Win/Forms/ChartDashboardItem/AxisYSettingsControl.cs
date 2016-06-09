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
using System.ComponentModel;
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.Native;
using DevExpress.DashboardCommon.Native;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors;
using System.Drawing;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class AxisYSettingsControl : DashboardUserControl {
		readonly Locker locker = new Locker();
		IChartYAxisSettingsHistoryItem historyItem;
		ChartAxisY CurrentChartAxisY { get { return (historyItem.PanesEnabled ? ((ChartAxisYSettings)cbPaneChooser.SelectedItem) : historyItem.Settings[0]).ChartAxisY; } }
		public AxisYSettingsControl() {
			InitializeComponent();
		}
		public void Initialize(IChartYAxisSettingsHistoryItem historyItem) {
			this.historyItem = historyItem;
			lciPaneChooser.Visibility = historyItem.PanesEnabled ? LayoutVisibility.Always : LayoutVisibility.Never;
			lciAlwaysShowZeroLevel.Visibility = historyItem.AlwaysShowZeroLevelEnabled ? LayoutVisibility.Always : LayoutVisibility.Never;
			if(historyItem.PanesEnabled) {
				foreach(ChartAxisYSettings settings in historyItem.Settings)
					cbPaneChooser.Properties.Items.Add(settings);
				cbPaneChooser.SelectedItem = historyItem.Settings[0];
			}
			UpdateSettingsState();
			UpdateSize();
		}
		void UpdateSettingsState() {
			ChartAxisY chartAxisY = CurrentChartAxisY;
			ceAlwaysShowZeroLevel.Checked = chartAxisY.AlwaysShowZeroLevel;
			ceReverse.Checked = chartAxisY.Reverse;
			ceShowGridLines.Checked = chartAxisY.ShowGridLines;
			ceTitleVisible.Checked = chartAxisY.TitleVisible;
			ceVisible.Checked = chartAxisY.Visible;
			ceLogarithmic.Checked = chartAxisY.Logarithmic;
			ceLogarithmicBase.SelectedIndex = (int)chartAxisY.LogarithmicBase;
			if (String.IsNullOrEmpty(chartAxisY.Title))
				rbDefaultTitleText.Checked = true;
			else
				rbCustomTitleText.Checked = true;
			UpdateControlsState();
			UpdateLogarithmicState();
		}
		void UpdateSize() {
			if(!lciPaneChooser.Visible)
				Height -= lciPaneChooser.Height;
			if(!lciAlwaysShowZeroLevel.Visible)
				Height -= lciAlwaysShowZeroLevel.Height;
		}
		void UpdateHistoryItem() {
			historyItem.UpdateSettings(CurrentChartAxisY);
		}
		void UpdateTitleTextControlState() {
			ChartAxisY chartAxisY = CurrentChartAxisY;
			teTitleText.Enabled = chartAxisY.TitleVisible && rbCustomTitleText.Checked;
			locker.Lock();
			try {
				teTitleText.Text = chartAxisY.DisplayTitle;
			}
			finally {
				locker.Unlock();
			}
		}
		void UpdateTitleControlsState() {
			bool visible = CurrentChartAxisY.TitleVisible;
			lcTitle.Enabled = visible;
			UpdateTitleTextControlState();
		}
		void UpdateControlsState() {
			bool visible = CurrentChartAxisY.Visible;
			ceTitleVisible.Enabled = visible;
			lcTitle.Enabled = visible;
			UpdateTitleControlsState();
		}
		void UpdateLogarithmicState() {
			ceLogarithmicBase.Enabled = CurrentChartAxisY.Logarithmic;
		}
		void OnVisibleCheckedChanged(object sender, EventArgs e) {
			CurrentChartAxisY.Visible = ceVisible.Checked;
			UpdateControlsState();
			UpdateHistoryItem();
		}
		void OnAlwaysShowZeroLevelCheckedChanged(object sender, EventArgs e) {
			CurrentChartAxisY.AlwaysShowZeroLevel = ceAlwaysShowZeroLevel.Checked;
			UpdateHistoryItem();
		}
		void OnShowGridLinesChanged(object sender, EventArgs e) {
			CurrentChartAxisY.ShowGridLines = ceShowGridLines.Checked;
			UpdateHistoryItem();
		}
		void OnTitleVisibleCheckedChanged(object sender, EventArgs e) {
			CurrentChartAxisY.TitleVisible = ceTitleVisible.Checked;
			UpdateTitleControlsState();
			UpdateHistoryItem();
		}
		void OnTitleDefaultTextCheckedChanged(object sender, EventArgs e) {
			CurrentChartAxisY.Title = rbDefaultTitleText.Checked ? String.Empty : teTitleText.Text;
			rbCustomTitleText.Checked = !rbDefaultTitleText.Checked;
			UpdateTitleTextControlState();
			UpdateHistoryItem();
		}
		void OnTitleCustomTextCheckedChanged(object sender, EventArgs e) {
			rbDefaultTitleText.Checked = !rbCustomTitleText.Checked;
		}
		void OnTitleTextEditValueChanged(object sender, EventArgs e) {
			if (!locker.IsLocked) {
				CurrentChartAxisY.Title = teTitleText.Text;
				UpdateHistoryItem();
			}
		}
		void OnSelectedAxisChanged(object sender, EventArgs e) {
			UpdateSettingsState();
		}
		void OnReverseCheckedChanged(object sender, EventArgs e) {
			CurrentChartAxisY.Reverse = ceReverse.Checked;
			UpdateHistoryItem();
		}
		void OnLogarithmicCheckedChanged(object sender, EventArgs e) {
			CurrentChartAxisY.Logarithmic = ceLogarithmic.Checked;
			UpdateLogarithmicState();
			UpdateHistoryItem();
		}
		void OnLogarithmicBaseSelectedIndexChanged(object sender, EventArgs e) {
			CurrentChartAxisY.LogarithmicBase = (LogarithmicBase)ceLogarithmicBase.SelectedIndex;
			UpdateHistoryItem();
		}
		void OnLogarithmicBaseDrawItem(object sender, ListBoxDrawItemEventArgs e) {
		}
	}
}
