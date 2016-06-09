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
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraEditors.Controls;
using System.Drawing;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartTitleControls {
	internal partial class TitleGeneralControl : ChartUserControl {
		DockableTitle title;
		AddTempTitleDelegate method;
		public TitleGeneralControl() {
			InitializeComponent();
		}
		void InitializeCore() {
			CheckEditHelper.SetCheckEditState(chVisible, title.Visibility);
			cbDock.SelectedIndex = (int)title.Dock;
			cbAlignment.SelectedIndex = (int)title.Alignment;
			txtIndent.EditValue = title.Indent;
			ceColor.EditValue = (Color)title.TextColor;
			txtFont.Text = GetFontText();
			CheckEditHelper.SetCheckEditState(chAntialize, title.EnableAntialiasing);
			cheWordWrap.Checked = title.WordWrap;
			speMaximumLinesCount.EditValue = title.MaxLineCount;
			UpdateControls();
		}
		void UpdateControls() {
			speMaximumLinesCount.Enabled = cheWordWrap.Checked;
			lblMaxLineCount.Enabled = cheWordWrap.Checked;
			bool isControlsEnabled = title.Visibility != Utils.DefaultBoolean.False;
			grPosition.Enabled = isControlsEnabled;
			grAppearance.Enabled = isControlsEnabled;
			chVisible.Enabled = true;
		}
		void AddTitle() {
			if (method != null)
				method(title);
		}
		void chVisible_CheckStateChanged(object sender, EventArgs e) {
			title.Visibility = CheckEditHelper.GetCheckEditState(chVisible);
			UpdateControls();
			AddTitle();
		}
		void cbDock_SelectedIndexChanged(object sender, EventArgs e) {
			title.Dock = (ChartTitleDockStyle)cbDock.SelectedIndex;
			AddTitle();
		}
		void cbAlignment_SelectedIndexChanged(object sender, EventArgs e) {
			title.Alignment = (StringAlignment)cbAlignment.SelectedIndex;
			AddTitle();
		}
		void txtIndent_EditValueChanged(object sender, EventArgs e) {
			title.Indent = Convert.ToInt32(txtIndent.EditValue);
			AddTitle();
		}
		void ceColor_EditValueChanged(object sender, EventArgs e) {
			title.TextColor = (Color)ceColor.EditValue;
			AddTitle();
		}
		void txtFont_QueryPopUp(object sender, CancelEventArgs e) {
			fontEditControl.FillFonts();
			fontEditControl.EditedFont = title.Font;
		}
		void fontEditControl_OnNeedClose(object sender, EventArgs e) {
			txtFont.ClosePopup();
		}
		void txtFont_QueryResultValue(object sender, QueryResultValueEventArgs e) {
			title.Font = fontEditControl.EditedFont;
			e.Value = GetFontText();
			AddTitle();
		}
		void chAntialize_CheckedChanged(object sender, EventArgs e) {
			title.EnableAntialiasing = CheckEditHelper.GetCheckEditState(chAntialize);
			AddTitle();
		}
		void fontEditControl_OnNeedClose_1(object sender, EventArgs e) {
			txtFont.ClosePopup();
		}
		void cheWordWrap_CheckedChanged(object sender, EventArgs e) {
			title.WordWrap = cheWordWrap.Checked;
			UpdateControls();
		}
		void speMaximumLinesCount_EditValueChanging(object sender, ChangingEventArgs e) {
			e.Cancel = Convert.ToInt32(e.NewValue) > 20;
		}
		void speMaximumLinesCount_EditValueChanged(object sender, EventArgs e) {
			title.MaxLineCount = Convert.ToInt32(speMaximumLinesCount.EditValue);
		}
		string GetFontText() {
			return String.Format(ChartLocalizer.GetString(ChartStringId.FontFormat), title.Font.Name, title.Font.Size, title.Font.Style);
		}
		public void Initialize(DockableTitle title, AddTempTitleDelegate method) {
			this.title = title;
			this.method = method;
			InitializeCore();
		}
	}
}
