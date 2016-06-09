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
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartLegendControls {
	internal partial class LegendGeneralControl : ChartUserControl {
		Chart chart;
		Legend legend;
		public LegendGeneralControl() {
			InitializeComponent();
		}
		public void Initialize(Chart chart) {
			this.chart = chart;
			this.legend = chart.Legend;
			if (legend.Direction == LegendDirection.LeftToRight || legend.Direction == LegendDirection.RightToLeft)
				chEquallySpacedItems.Checked = legend.EquallySpacedItems;
			chUseCheckBoxes.Checked = legend.UseCheckBoxes;
			cbDirection.SelectedIndex = (int)legend.Direction;
			cbHAlignment.SelectedIndex = (int)legend.AlignmentHorizontal;
			cbVAlignment.SelectedIndex = (int)legend.AlignmentVertical;
			txtHLimits.EditValue = legend.MaxHorizontalPercentage;
			txtVLimits.EditValue = legend.MaxVerticalPercentage;
			marginsControl.Initialize(legend.Margins);
			UpdateControls();
		}
		void UpdateControls() {
			chEquallySpacedItems.Enabled = legend.Direction == LegendDirection.LeftToRight ||
				legend.Direction == LegendDirection.RightToLeft;
			if (legend.Direction == LegendDirection.LeftToRight || legend.Direction == LegendDirection.RightToLeft)
				legend.EquallySpacedItems = chEquallySpacedItems.Checked;
			grLimits.Enabled = !PivotGridDataSourceUtils.HasDataSource(chart.DataContainer.PivotGridDataSourceOptions) || !chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled;
		}
		void cbDirection_SelectedIndexChanged(object sender, EventArgs e) {
			legend.Direction = (LegendDirection)cbDirection.SelectedIndex;
			UpdateControls();
		}
		void cbVAlignment_SelectedIndexChanged(object sender, EventArgs e) {
			legend.AlignmentVertical = (LegendAlignmentVertical)cbVAlignment.SelectedIndex;
		}
		void cbHAlignment_SelectedIndexChanged(object sender, EventArgs e) {
			legend.AlignmentHorizontal = (LegendAlignmentHorizontal)cbHAlignment.SelectedIndex;
		}
		void txtVLimits_EditValueChanged(object sender, EventArgs e) {
			legend.MaxVerticalPercentage = Convert.ToInt32(txtVLimits.EditValue);
		}
		void txtHLimits_EditValueChanged(object sender, EventArgs e) {
			legend.MaxHorizontalPercentage = Convert.ToInt32(txtHLimits.EditValue);
		}
		void chEquallySpacedItems_CheckedChanged(object sender, EventArgs e) {
			legend.EquallySpacedItems = chEquallySpacedItems.Checked;
		}
		void chUseCheckBoxes_CheckedChanged(object sender, EventArgs e) {
			legend.UseCheckBoxes = chUseCheckBoxes.Checked;
		}
	}
}
