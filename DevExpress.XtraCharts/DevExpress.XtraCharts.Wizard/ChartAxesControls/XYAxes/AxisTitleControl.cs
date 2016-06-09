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
using System.ComponentModel;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class AxisTitleControl : ChartUserControl {
		AxisTitle title;
		AxisBase axis;
		Chart chart;
		public AxisTitleControl() {
			InitializeComponent();
		}
		public void Initialize(UserLookAndFeel lookAndFeel, AxisTitle title, AxisBase axis, Chart chart) {
			this.title = title;
			this.axis = axis;
			this.chart = chart;
			fontEditControl.SetLookAndFeel(lookAndFeel);
			this.cbAlignment.SelectedIndex = (int)title.Alignment;
			CheckEditHelper.SetCheckEditState(this.chAntialize, title.EnableAntialiasing);
			CheckEditHelper.SetCheckEditState(chVisible, title.Visibility);
			this.txtText.EditValue = title.Text;
			this.ceTextColor.EditValue = title.TextColor;
			this.txtFont.Text = GetFontText();
			UpdateControls();
		}
		void UpdateControls() {
			bool isControlsVisible = title.Visibility != Utils.DefaultBoolean.False;
			chAntialize.Enabled = isControlsVisible;
			bool isAutoLayoutSettingsEnabled = PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(chart.DataContainer.PivotGridDataSourceOptions, axis, true);
			pnlText.Enabled = isControlsVisible && !isAutoLayoutSettingsEnabled;
			pnlAlignment.Enabled = isControlsVisible;
			pnlColor.Enabled = isControlsVisible;
			pnlFont.Enabled = isControlsVisible;
		}
		void chVisible_CheckStateChanged(object sender, EventArgs e) {
			title.Visibility = CheckEditHelper.GetCheckEditState(chVisible);
			UpdateControls();
		}
		void chAntialize_CheckedChanged(object sender, EventArgs e) {
			this.title.EnableAntialiasing = CheckEditHelper.GetCheckEditState(this.chAntialize);
		}
		void txtText_EditValueChanged(object sender, EventArgs e) {
			this.title.Text = this.txtText.EditValue.ToString();
		}
		void txtFont_QueryResultValue(object sender, QueryResultValueEventArgs e) {
			this.title.Font = this.fontEditControl.EditedFont;
			e.Value = GetFontText();
		}
		void ceTextColor_EditValueChanged(object sender, EventArgs e) {
			this.title.TextColor = (Color)this.ceTextColor.EditValue;
		}
		void txtFont_QueryPopUp(object sender, CancelEventArgs e) {
			fontEditControl.FillFonts();
			fontEditControl.EditedFont = this.title.Font;
		}
		void fontEditControl_OnNeedClose(object sender, EventArgs e) {
			txtFont.ClosePopup();
		}
		void cbAlignment_SelectedIndexChanged(object sender, EventArgs e) {
			this.title.Alignment = (StringAlignment)cbAlignment.SelectedIndex;
		}
		string GetFontText() {
			return String.Format(ChartLocalizer.GetString(ChartStringId.FontFormat),
					this.title.Font.Name, this.title.Font.Size,
					this.title.Font.Style);
		}
	}
}
