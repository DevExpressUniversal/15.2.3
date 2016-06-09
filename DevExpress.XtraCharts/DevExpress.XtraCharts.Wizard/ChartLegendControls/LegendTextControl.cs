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
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartLegendControls {
	internal partial class LegendTextControl : ChartUserControl {
		Legend legend;
		public LegendTextControl() {
			InitializeComponent();
		}
		string GetFontText() {
			return String.Format(ChartLocalizer.GetString(ChartStringId.FontFormat), legend.Font.Name, legend.Font.Size, legend.Font.Style);
		}
		void chAntialize_CheckedChanged(object sender, EventArgs e) {
			legend.EnableAntialiasing = CheckEditHelper.GetCheckEditState(chAntialize);
		}
		void ceTextColor_EditValueChanged(object sender, EventArgs e) {
			legend.TextColor = (Color)ceTextColor.EditValue;
		}
		void txtFont_QueryPopUp(object sender, CancelEventArgs e) {
			fontEditControl.FillFonts();
			fontEditControl.EditedFont = legend.Font;
		}
		void txtFont_QueryResultValue(object sender, DevExpress.XtraEditors.Controls.QueryResultValueEventArgs e) {
			legend.Font = fontEditControl.EditedFont;
			e.Value = GetFontText();
		}
		void fontEditControl_OnNeedClose(object sender, EventArgs e) {
			txtFont.ClosePopup();
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			UpdateControls();
			legend.TextVisible = chVisible.Checked;
		}
		void UpdateControls() {
			chAntialize.Enabled = chVisible.Checked;
			ceTextColor.Enabled = chVisible.Checked;
			txtFont.Enabled = chVisible.Checked;
			lblColor.Enabled = chVisible.Checked;
			lblFont.Enabled = chVisible.Checked;
		}
		public void Initialize(UserLookAndFeel lookAndFeel, Legend legend) {
			this.legend = legend;
			fontEditControl.SetLookAndFeel(lookAndFeel);
			CheckEditHelper.SetCheckEditState(chAntialize, legend.EnableAntialiasing);
			ceTextColor.EditValue = legend.TextColor;
			txtFont.Text = GetFontText();
			chVisible.Checked = legend.TextVisible;
			UpdateControls();
		}
	}
}
