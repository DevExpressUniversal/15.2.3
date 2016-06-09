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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class AxisLabelTextSettingsControl : ChartUserControl {
		AxisLabel label;
		public AxisLabelTextSettingsControl() {
			InitializeComponent();
		}
		public void Initialize(UserLookAndFeel lookAndFeel, AxisLabel label) {
			this.label = label;
			fontEditControl.SetLookAndFeel(lookAndFeel);
			CheckEditHelper.SetCheckEditState(ceAntialiasing, label.EnableAntialiasing);
			ceColor.EditValue = label.TextColor;
			pceFont.Text = GetFontText();
			speMaxWidth.Value = label.MaxWidth;
			speMaxLineCount.Value = label.MaxLineCount;
			cbAlignment.SelectedIndex = (int)label.TextAlignment;
		}
		string GetFontText() {
			Font font = label.Font;
			return String.Format(ChartLocalizer.GetString(ChartStringId.FontFormat), font.Name, font.Size, font.Style);
		}
		void ceColor_EditValueChanged(object sender, EventArgs e) {
			label.TextColor = (Color)ceColor.EditValue;
		}
		void pceFont_QueryPopUp(object sender, CancelEventArgs e) {
			fontEditControl.FillFonts();
			fontEditControl.EditedFont = label.Font;
		}
		void pceFont_QueryResultValue(object sender, QueryResultValueEventArgs e) {
			label.Font = fontEditControl.EditedFont;
			e.Value = GetFontText();
		}
		void fontEditControl_OnNeedClose(object sender, EventArgs e) {
			pceFont.ClosePopup();
		}
		void speMaxWidth_EditValueChanged(object sender, EventArgs e) {
			label.MaxWidth = (int)speMaxWidth.Value;
		}
		void speMaxLineCount_EditValueChanged(object sender, EventArgs e) {
			label.MaxLineCount = (int)speMaxLineCount.Value;
		}
		void cbAlignment_SelectedIndexChanged(object sender, EventArgs e) {
			label.TextAlignment = (StringAlignment)cbAlignment.SelectedIndex;
		}
		private void ceAntialiasing_CheckStateChanged(object sender, EventArgs e) {
			label.EnableAntialiasing = CheckEditHelper.GetCheckEditState(ceAntialiasing);
		}
	}
}
