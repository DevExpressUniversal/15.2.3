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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTab;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class PaneControl : PaneControlBase {
		public override XtraTabControl TabControl { get { return tbcTabPages; } } 
		public PaneControl() {
			InitializeComponent();
			InitializeTags();
		}
		void UpdateSizeControls() {
			if(Pane.SizeMode == PaneSizeMode.UseWeight) {
				panelWeight.Visible = true;
				panelSizeInPixels.Visible = false;
				txtWeight.EditValue = Math.Round(Pane.Weight, 1);
			}
			else {
				panelWeight.Visible = false;
				panelSizeInPixels.Visible = true;
				txtSizeInPixels.EditValue = Pane.SizeInPixels;
			}
		}
		void UpdateBorderControls() {
			cbBorderVisible.Checked = Pane.BorderVisible;
			lblColor.Enabled = Pane.BorderVisible;
			ceBorderColor.Enabled = Pane.BorderVisible;
			ceBorderColor.EditValue = Pane.BorderColor;
		}
		void UpdateScrollBarOptions() {
			scrollBarOptionsControl.Enabled = Pane.ActualEnableAxisXScrolling || Pane.ActualEnableAxisYScrolling;
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			Pane.Visible = chVisible.Checked;
		}
		void txtName_Validating(object sender, CancelEventArgs e) {
			if (txtName.EditValue.ToString() == "") {
				e.Cancel = true;
				txtName.ErrorText = ChartLocalizer.GetString(ChartStringId.MsgEmptyPaneName);
			}
			else {
				XYDiagramPane p = Pane as XYDiagramPane;
				if (p != null) {
					p.Name = txtName.EditValue.ToString();
					ChangePaneNameMethod();
				}
			}
		}
		void cbSizeMode_SelectedIndexChanged(object sender, EventArgs e) {
			Pane.SizeMode = (PaneSizeMode)cbSizeMode.SelectedIndex;
			UpdateSizeControls();
		}
		void txtWeight_EditValueChanged(object sender, EventArgs e) {
			Pane.Weight = Convert.ToDouble(txtWeight.EditValue);
		}
		void txtSizeInPixels_EditValueChanged(object sender, EventArgs e) {
			Pane.SizeInPixels = Convert.ToInt32(txtSizeInPixels.EditValue);		
		}
		void cbBorderVisible_CheckedChanged(object sender, EventArgs e) {
			Pane.BorderVisible = cbBorderVisible.Checked;
			UpdateBorderControls();
		}
		void chBorderColor_EditValueChanged(object sender, EventArgs e) {
			Pane.BorderColor = (Color)ceBorderColor.EditValue;
		}
		void chEnableAxisXScrolling_CheckStateChanged(object sender, EventArgs e) {
			Pane.EnableAxisXScrolling = CheckEditHelper.GetCheckEditState(chEnableAxisXScrolling);
			chEnableAxisXScrolling.Text = Pane.ActualEnableAxisXScrolling ?
				ChartLocalizer.GetString(ChartStringId.WizEnableScrollingTrue) :
				ChartLocalizer.GetString(ChartStringId.WizEnableScrollingFalse);
			UpdateScrollBarOptions();
		}
		void chEnableAxisXZooming_CheckStateChanged(object sender, EventArgs e) {
			Pane.EnableAxisXZooming = CheckEditHelper.GetCheckEditState(chEnableAxisXZooming);
			chEnableAxisXZooming.Text = Pane.ActualEnableAxisXZooming ?
				ChartLocalizer.GetString(ChartStringId.WizEnableZoomingTrue) :
				ChartLocalizer.GetString(ChartStringId.WizEnableZoomingFalse);
		}
		void chEnableAxisYScrolling_CheckStateChanged(object sender, EventArgs e) {
			Pane.EnableAxisYScrolling = CheckEditHelper.GetCheckEditState(chEnableAxisYScrolling);
			chEnableAxisYScrolling.Text = Pane.ActualEnableAxisYScrolling ?
				ChartLocalizer.GetString(ChartStringId.WizEnableScrollingTrue) :
				ChartLocalizer.GetString(ChartStringId.WizEnableScrollingFalse);
			UpdateScrollBarOptions();
		}
		void chEnableAxisYZooming_CheckStateChanged(object sender, EventArgs e) {
			Pane.EnableAxisYZooming = CheckEditHelper.GetCheckEditState(chEnableAxisYZooming);
			chEnableAxisYZooming.Text = Pane.ActualEnableAxisYZooming ?
				ChartLocalizer.GetString(ChartStringId.WizEnableZoomingTrue) :
				ChartLocalizer.GetString(ChartStringId.WizEnableZoomingFalse);
		}
		protected override void InitializeTags() {
			tbGeneral.Tag = PanePageTab.General;
			tbAppearance.Tag = PanePageTab.Appearance;
			tbBorder.Tag = PanePageTab.Border;
			tbShadow.Tag = PanePageTab.Shadow;
			tbScrollingZooming.Tag = PanePageTab.ScrollingZooming;
			tbScrollBarOptions.Tag = PanePageTab.ScrollBarOptions;
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			chVisible.Checked = Pane.Visible;
			panelName.Enabled = Pane is XYDiagramPane;
			txtName.EditValue = Pane.Name;
			cbSizeMode.SelectedIndex = (int)Pane.SizeMode;
			UpdateSizeControls();
			backgroundControl.Initialize(Pane, OriginalChart);
			UpdateBorderControls();
			shadowControl.Initialize(Pane.Shadow);
			if (OriginalChart != null && OriginalChart.Container != null && OriginalChart.Container.ControlType == ChartContainerType.WebControl) {
				tbcTabPages.TabPages.Remove(tbScrollingZooming);
				tbcTabPages.TabPages.Remove(tbScrollBarOptions);
			}
			else {
				scrollBarOptionsControl.Initialize(Pane);
				CheckEditHelper.SetCheckEditState(chEnableAxisXScrolling, Pane.EnableAxisXScrolling);
				CheckEditHelper.SetCheckEditState(chEnableAxisXZooming, Pane.EnableAxisXZooming);
				CheckEditHelper.SetCheckEditState(chEnableAxisYScrolling, Pane.EnableAxisYScrolling);
				CheckEditHelper.SetCheckEditState(chEnableAxisYZooming, Pane.EnableAxisYZooming);
				UpdateScrollBarOptions();
			}
		}
	}
}
