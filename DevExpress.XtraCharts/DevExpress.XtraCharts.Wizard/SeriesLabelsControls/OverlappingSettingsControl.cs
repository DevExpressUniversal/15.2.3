#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	internal partial class OverlappingSettingsControl : DevExpress.XtraCharts.Wizard.ChartUserControl {
		struct ResolveOverlappingModeItem {
			readonly ResolveOverlappingMode mode;
			readonly string name;
			public ResolveOverlappingMode Mode { get { return mode; } }
			public string Name { get { return name; } }
			public ResolveOverlappingModeItem(ResolveOverlappingMode mode) {
				this.mode = mode;
				switch (mode) {
					case ResolveOverlappingMode.None:
						name = ChartLocalizer.GetString(ChartStringId.WizResolveOverlappingModeNone);
						break;
					case ResolveOverlappingMode.Default:
						name = ChartLocalizer.GetString(ChartStringId.WizResolveOverlappingModeDefault);
						break;
					case ResolveOverlappingMode.HideOverlapped:
						name = ChartLocalizer.GetString(ChartStringId.WizResolveOverlappingModeHideOverlapping);
						break;
					case ResolveOverlappingMode.JustifyAllAroundPoint:
						name = ChartLocalizer.GetString(ChartStringId.WizResolveOverlappingModeJustifyAllAroundPoints);
						break;
					case ResolveOverlappingMode.JustifyAroundPoint:
						name = ChartLocalizer.GetString(ChartStringId.WizResolveOverlappingModeJustifyAroundPoint);
						break;
					default:
						ChartDebug.Fail("Unknown resolve overlapping mode.");
						name = ChartLocalizer.GetString(ChartStringId.WizResolveOverlappingModeDefault);
						break;
				}
			}
			public override string ToString() {
				return name;
			}
			public override bool Equals(object obj) {
				if (!(obj is ResolveOverlappingModeItem))
					return false;
				ResolveOverlappingModeItem item = (ResolveOverlappingModeItem)obj;
				return item.mode == mode;
			}
			public override int GetHashCode() {
				return mode.GetHashCode();
			}
		}
		SeriesLabelBase label;
		SeriesBase series;
		Chart chart;
		ILabelBehaviorProvider LabelBehaviorProvider { get { return label; } }
		public OverlappingSettingsControl() {
			InitializeComponent();
		}
		void txtIndent_EditValueChanged(object sender, EventArgs e) {
			label.ResolveOverlappingMinIndent = Convert.ToInt32(spinIndent.EditValue);
		}
		void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e) {
			label.ResolveOverlappingMode = ((ResolveOverlappingModeItem)comboBoxMode.SelectedItem).Mode;
			UpdateControls();
		}
		public void UpdateControls() {
			if (label != null) {
				chartPanelIndent.Enabled = label.ResolveOverlappingMode != ResolveOverlappingMode.None && comboBoxMode.Properties.Items.Count > 1;
				chartPanelMode.Enabled = !PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(chart.DataContainer.PivotGridDataSourceOptions, series);
			}
		}
		public void Initialize(SeriesLabelBase label, SeriesBase series, Chart chart) {
			this.label = label;
			this.series = series;
			this.chart = chart;
			comboBoxMode.Properties.Items.BeginUpdate();
			comboBoxMode.Properties.Items.Clear();
			foreach(ResolveOverlappingMode mode in Enum.GetValues(typeof(ResolveOverlappingMode)))
				if (LabelBehaviorProvider.CheckResolveOverlappingMode(mode))
					comboBoxMode.Properties.Items.Add(new ResolveOverlappingModeItem(mode));
			comboBoxMode.SelectedItem = new ResolveOverlappingModeItem(label.ResolveOverlappingMode);
			comboBoxMode.Properties.Items.EndUpdate();
			if(comboBoxMode.SelectedItem == null && comboBoxMode.Properties.Items.Count > 0)
				comboBoxMode.SelectedIndex = 0;
			spinIndent.EditValue = label.ResolveOverlappingMinIndent;
			UpdateControls();
		}
	}
}
