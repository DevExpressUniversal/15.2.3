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
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class ConstantLineTitleControl : ChartUserControl {
		ConstantLineTitle title;
		bool Locked { get { return title == null; } }
		public ConstantLineTitleControl() {
			InitializeComponent();
		}
		public void Initialize(UserLookAndFeel lookAndFeel, ConstantLine line) {
			this.title = line != null ? line.Title : null;
			fontEditControl.SetLookAndFeel(lookAndFeel);
			this.Enabled = title != null;
			if (title != null) {
				CheckEditHelper.SetCheckEditState(this.chAntialize, title.EnableAntialiasing);
				this.chVisible.Checked = title.Visible;
				this.pnlTextSettings.Enabled = title.Visible;
				this.chShowBelow.Enabled = title.Visible;
				this.txtText.EditValue = title.Text;
				this.ceTextColor.EditValue = title.TextColor;
				this.txtFont.Text = GetFontText();
				this.cbAlignment.SelectedIndex = (int)title.Alignment;
				this.chShowBelow.Checked = title.ShowBelowLine;
			}
			else {
				this.chAntialize.Checked = false;
				this.chVisible.Checked = false;
				this.pnlTextSettings.Enabled = false;
				this.txtText.EditValue = "";
				this.ceTextColor.EditValue = "";
				this.txtFont.Text = "";
			}
		}
		string GetFontText() {
			return String.Format(ChartLocalizer.GetString(ChartStringId.FontFormat),
					this.title.Font.Name, this.title.Font.Size,
					this.title.Font.Style);
		}
		private void chVisible_CheckedChanged(object sender, EventArgs e) {
			if (Locked)
				return;
			this.title.Visible = this.chVisible.Checked;
			this.pnlTextSettings.Enabled = this.chVisible.Checked;
			chShowBelow.Enabled = this.chVisible.Checked;
		}
		private void chAntialize_CheckedChanged(object sender, EventArgs e) {
			if (Locked)
				return;
			this.title.EnableAntialiasing = CheckEditHelper.GetCheckEditState(this.chAntialize);
		}
		private void txtText_EditValueChanged(object sender, EventArgs e) {
			if (Locked)
				return;
			this.title.Text = this.txtText.EditValue.ToString();
		}
		private void txtFont_QueryResultValue(object sender, DevExpress.XtraEditors.Controls.QueryResultValueEventArgs e) {
			if (Locked)
				return;
			this.title.Font = this.fontEditControl.EditedFont;
			e.Value = GetFontText();
		}
		private void ceTextColor_EditValueChanged(object sender, EventArgs e) {
			if (Locked)
				return;
			this.title.TextColor = (Color)this.ceTextColor.EditValue;
		}
		private void txtFont_QueryPopUp(object sender, CancelEventArgs e) {
			if (Locked)
				return;
			fontEditControl.FillFonts();
			fontEditControl.EditedFont = this.title.Font;
		}
		private void fontEditControl_OnNeedClose(object sender, EventArgs e) {
			if (Locked)
				return;
			txtFont.ClosePopup();
		}
		private void chShowBelow_CheckedChanged(object sender, EventArgs e) {
			if (Locked)
				return;
			this.title.ShowBelowLine = chShowBelow.Checked;
		}
		private void cbAlignment_SelectedIndexChanged(object sender, EventArgs e) {
			if (Locked)
				return;
			this.title.Alignment = (ConstantLineTitleAlignment)this.cbAlignment.SelectedIndex;
		}
	}
}
