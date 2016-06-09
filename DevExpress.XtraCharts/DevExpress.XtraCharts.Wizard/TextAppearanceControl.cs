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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class TextAppearanceControl : ChartUserControl {
		ITextAppearance textAppearance;
		Action0 fontChangedCallback;
		public TextAppearanceControl() {
			InitializeComponent();
		}
		string GetFontText() {
			return String.Format(ChartLocalizer.GetString(ChartStringId.FontFormat), textAppearance.Font.Name, textAppearance.Font.Size, textAppearance.Font.Style);
		}
		public void Initialize(ITextAppearance textAppearance) {
			Initialize(textAppearance, null);
		}
		public void Initialize(ITextAppearance textAppearance, Action0 fontChangedCallback) {
			this.textAppearance = textAppearance;
			this.fontChangedCallback = fontChangedCallback;
			clrColor.EditValue = textAppearance.TextColor;
			CheckEditHelper.SetCheckEditState(chAntialiasing, textAppearance.EnableAntialiasing);
			popFont.Text = GetFontText();
		}
		void popFont_QueryPopUp(object sender, CancelEventArgs e) {
			fontEditControl.FillFonts();
			fontEditControl.EditedFont = textAppearance.Font;
		}
		void fontEditControl_OnNeedClose(object sender, EventArgs e) {
			popFont.ClosePopup();
		}
		void popFont_QueryResultValue(object sender, QueryResultValueEventArgs e) {
			textAppearance.Font = fontEditControl.EditedFont;
			e.Value = GetFontText();
			if (fontChangedCallback != null)
				fontChangedCallback();
		}
		void clrColor_ColorChanged(object sender, EventArgs e) {
			textAppearance.TextColor = clrColor.Color;
		}
		void chAntialiasing_CheckStateChanged(object sender, EventArgs e) {
			textAppearance.EnableAntialiasing = CheckEditHelper.GetCheckEditState(chAntialiasing);
		}
	}
}
