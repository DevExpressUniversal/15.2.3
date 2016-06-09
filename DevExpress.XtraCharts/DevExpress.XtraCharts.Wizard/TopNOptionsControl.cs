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
using System.Globalization;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class TopNOptionsControl : ChartUserControl {
		TopNOptions topNOptions;
		public TopNOptionsControl() {
			InitializeComponent();
		}
		public void Initialize(TopNOptions topNOptions, bool shouldModifyOthers) {
			this.topNOptions = topNOptions;
			UpdateControls();
			pnlOthers.Visible = shouldModifyOthers;
		}
		void UpdateControls() {
			SuspendLayout();
			try {
				cbMode.SelectedIndex = (int)topNOptions.Mode;
				pnlCount.Visible = topNOptions.Mode == TopNMode.Count;
				pnlThresholdValue.Visible = topNOptions.Mode == TopNMode.ThresholdValue;
				pnlThresholdPercent.Visible = topNOptions.Mode == TopNMode.ThresholdPercent;
				txtCount.EditValue = topNOptions.Count;
				txtThresholdValue.EditValue = topNOptions.ThresholdValue;
				txtThresholdPercent.EditValue = topNOptions.ThresholdPercent;
				chShowOthers.Checked = topNOptions.ShowOthers;
				txtOthersArgument.Enabled = topNOptions.ShowOthers;
				txtOthersArgument.EditValue = topNOptions.OthersArgument;
			}
			finally {
				ResumeLayout();
			}
		}
		void cbMode_SelectedIndexChanged(object sender, EventArgs e) {
			topNOptions.Mode = (TopNMode)cbMode.SelectedIndex;
			UpdateControls();
		}
		void txtCount_EditValueChanged(object sender, EventArgs e) {
			topNOptions.Count = Convert.ToInt32(txtCount.EditValue);
		}
		void txtThresholdValue_EditValueChanging(object sender, ChangingEventArgs e) {
			if (Convert.ToDouble(e.NewValue, CultureInfo.InvariantCulture) <= 0)
				e.Cancel = true;
		}
		void txtThresholdValue_EditValueChanged(object sender, EventArgs e) {
			topNOptions.ThresholdValue = Convert.ToDouble(txtThresholdValue.EditValue);
		}
		void txtThresholdPercent_EditValueChanging(object sender, ChangingEventArgs e) {
			double percent = Convert.ToDouble(e.NewValue, CultureInfo.InvariantCulture);
			if (percent <= 0.0 || percent >= 100.0)
				e.Cancel = true;
		}
		void txtThresholdPercent_EditValueChanged(object sender, EventArgs e) {
			topNOptions.ThresholdPercent = Convert.ToDouble(txtThresholdPercent.EditValue);
		}
		void chShowOthers_CheckedChanged(object sender, EventArgs e) {
			topNOptions.ShowOthers = chShowOthers.Checked;
			UpdateControls();
		}
		void txtOthersArgument_EditValueChanged(object sender, EventArgs e) {
			topNOptions.OthersArgument = (string)txtOthersArgument.EditValue;
		}
	}
}
