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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class FibonacciIndicatorControl : IndicatorControlBase {
		readonly string fibonacciArcsName = ChartLocalizer.GetString(ChartStringId.FibonacciArcs);
		readonly string fibonacciFansName = ChartLocalizer.GetString(ChartStringId.FibonacciFans);
		readonly string fibonacciRetracementName = ChartLocalizer.GetString(ChartStringId.FibonacciRetracement);
		FibonacciIndicator FibonacciIndicator { get { return (FibonacciIndicator)Indicator; } }
		public FibonacciIndicatorControl() {
			InitializeComponent();
		}
		public override void Initialize(Indicator indicator) {
			base.Initialize(indicator);
			FibonacciIndicator fibonacciIndicator = FibonacciIndicator;
			cbKind.Properties.Items.Clear();
			cbKind.Properties.Items.Add(fibonacciArcsName);
			cbKind.Properties.Items.Add(fibonacciFansName);
			cbKind.Properties.Items.Add(fibonacciRetracementName);
			switch (fibonacciIndicator.Kind) {
				case FibonacciIndicatorKind.FibonacciArcs:
					cbKind.SelectedItem = fibonacciArcsName;
					break;
				case FibonacciIndicatorKind.FibonacciFans:
					cbKind.SelectedItem = fibonacciFansName;
					break;
				case FibonacciIndicatorKind.FibonacciRetracement:
					cbKind.SelectedItem = fibonacciRetracementName;
					break;
			}
			InitializeLevels();
			financialIndicatorControl.Initialize(fibonacciIndicator);
			ceLevelsColor.EditValue = fibonacciIndicator.Color;
			levelsLineStyleControl.Initialize(fibonacciIndicator.LineStyle);
			ceBaseLevelsColor.EditValue = fibonacciIndicator.BaseLevelColor;
			baseLevelsLineStyleControl.Initialize(fibonacciIndicator.BaseLevelLineStyle);
			chkLabelsVisible.Checked = fibonacciIndicator.Label.Visible;
			CheckEditHelper.SetCheckEditState(chkLabelsAntialiasing, fibonacciIndicator.Label.EnableAntialiasing);
			ceLabelsColor.EditValue = fibonacciIndicator.Label.TextColor;
			ceBaseLevelLabelsColor.EditValue = fibonacciIndicator.Label.BaseLevelTextColor;
			txtLabelsFont.Text = GetFontText();
		}
		protected override void UpdateControls() {
			base.UpdateControls();
			xtraTabControl.Enabled = Indicator.Visible;
		}
		void InitializeLevels() {		   
			FibonacciIndicator fibonacciIndicator = FibonacciIndicator;
			IFibonacciIndicatorBehavior indicatorBehavior = fibonacciIndicator;
			chLevel0.Enabled = indicatorBehavior.ShowLevel0PropertyEnabled;
			chLevel0.Checked = fibonacciIndicator.ShowLevel0;
			chLevel23_6.Checked = fibonacciIndicator.ShowLevel23_6;
			chLevel76_4.Checked = fibonacciIndicator.ShowLevel76_4;
			chLevel100.Enabled = indicatorBehavior.ShowLevel100PropertyEnabled;
			chLevel100.Checked = fibonacciIndicator.ShowLevel100;
			chAdditionalLevels.Enabled = indicatorBehavior.ShowAdditionalLevelsPropertyEnabled;
			chAdditionalLevels.Checked = fibonacciIndicator.ShowAdditionalLevels;
		}
		string GetFontText() {
			Font font = FibonacciIndicator.Label.Font;
			return String.Format(ChartLocalizer.GetString(ChartStringId.FontFormat), font.Name, font.Size, font.Style);
		}
		void cbKind_SelectedIndexChanged(object sender, EventArgs e) {
			string name = cbKind.SelectedItem.ToString();
			if (name == fibonacciFansName)
				FibonacciIndicator.Kind = FibonacciIndicatorKind.FibonacciFans;
			else if (name == fibonacciRetracementName)
				FibonacciIndicator.Kind = FibonacciIndicatorKind.FibonacciRetracement;
			else
				FibonacciIndicator.Kind = FibonacciIndicatorKind.FibonacciArcs;
			InitializeLevels();
		}
		void chkLevel0_CheckedChanged(object sender, EventArgs e) {
			FibonacciIndicator.ShowLevel0 = chLevel0.Checked;
		}
		void chkLevel23_6_CheckedChanged(object sender, EventArgs e) {
			FibonacciIndicator.ShowLevel23_6 = chLevel23_6.Checked;
		}
		void chkLevel76_4_CheckedChanged(object sender, EventArgs e) {
			FibonacciIndicator.ShowLevel76_4 = chLevel76_4.Checked;
		}
		void chkLevel100_CheckedChanged(object sender, EventArgs e) {
			FibonacciIndicator.ShowLevel100 = chLevel100.Checked;
		}
		void chkAdditionalLevels_CheckedChanged(object sender, EventArgs e) {
			FibonacciIndicator.ShowAdditionalLevels = chAdditionalLevels.Checked;
		}
		void ceLevelsColor_EditValueChanged(object sender, EventArgs e) {
			FibonacciIndicator.Color = (Color)ceLevelsColor.EditValue;
		}
		void ceBaseLevelsColor_EditValueChanged(object sender, EventArgs e) {
			FibonacciIndicator.BaseLevelColor = (Color)ceBaseLevelsColor.EditValue;
		}
		void chkLabelsVisible_CheckedChanged(object sender, EventArgs e) {
			bool visible = chkLabelsVisible.Checked;
			FibonacciIndicator.Label.Visible = visible;
			chkLabelsAntialiasing.Enabled = visible;
			labelLabelsColor.Enabled = visible;
			ceLabelsColor.Enabled = visible;
			labelBaseLevelLabelsColor.Enabled = visible;
			ceBaseLevelLabelsColor.Enabled = visible;
			labelLabelsFont.Enabled = visible;
			txtLabelsFont.Enabled = visible;
		}
		void chkLabelsAntialiasing_CheckedChanged(object sender, EventArgs e) {
			FibonacciIndicator.Label.EnableAntialiasing = CheckEditHelper.GetCheckEditState(chkLabelsAntialiasing);
		}
		void ceLabelsColor_EditValueChanged(object sender, EventArgs e) {
			FibonacciIndicator.Label.TextColor = (Color)ceLabelsColor.EditValue;
		}
		void ceBaseLevelLabelsColor_EditValueChanged(object sender, EventArgs e) {
			FibonacciIndicator.Label.BaseLevelTextColor = (Color)ceBaseLevelLabelsColor.EditValue;
		}
		void txtLabelsFont_QueryPopUp(object sender, CancelEventArgs e) {
			fontEditControl.FillFonts();
			fontEditControl.EditedFont = FibonacciIndicator.Label.Font;
		}
		void txtLabelsFont_QueryResultValue(object sender, QueryResultValueEventArgs e) {
			FibonacciIndicator.Label.Font = fontEditControl.EditedFont;
			e.Value = GetFontText();
		}
	}
}
