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
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class ConstantLineGeneralControl : ValidateControl {
		ConstantLine line;
		IAxisData axis;
		Chart Chart {
			get {
				if (line == null)
					return null;
				else
					return ((IOwnedElement)line).Owner.Owner.Owner as Chart;
			}
		}
		public event MethodInvoker UpdateByContent;
		public ConstantLineGeneralControl() {
			InitializeComponent();
		}
		void OnUpdateByContent() {
			if (UpdateByContent != null)
				UpdateByContent();
		}
		void UpdateControls() {
			if (Chart == null)
				return;
			pnlControls.Enabled = line.Visible;
			pnlValue.Enabled = line.Visible;
			pnlLegendText.Enabled = line.Visible && line.ShowInLegend;
			chLegendVisible.Enabled = line.Visible;
			chCheckableInLegend.Enabled = line.Visible && line.ShowInLegend && Chart.Legend.UseCheckBoxes;
			chCheckedInLegend.Enabled = chCheckableInLegend.Enabled && line.CheckableInLegend;
		}
		void chLegendVisible_CheckedChanged(object sender, EventArgs e) {
			if (line == null)
				return;
			line.ShowInLegend = chLegendVisible.Checked;
			UpdateControls();
		}
		void txtLegendText_EditValueChanged(object sender, EventArgs e) {
			if (line == null)
				return;
			line.LegendText = txtLegendText.EditValue.ToString();
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			if (line == null)
				return;
			line.Visible = chVisible.Checked;
			UpdateControls();
		}
		void chShowBehind_CheckedChanged(object sender, EventArgs e) {
			if (line == null)
				return;
			line.ShowBehind = chShowBehind.Checked;
		}
		void txtName_Validating(object sender, CancelEventArgs e) {
			if (line == null)
				return;
			SetInvalidState();
			if (txtName.EditValue == null)
				e.Cancel = true;
		}
		void txtName_Validated(object sender, EventArgs e) {
			if (line == null)
				return;
			line.Name = (string)txtName.EditValue;
			OnUpdateByContent();
			SetValidState();
		}
		void txtValue_Validating(object sender, CancelEventArgs e) {
			if (line == null)
				return;
			SetInvalidState();
			if (!String.IsNullOrEmpty((string)txtValue.Text)) {
				object nativeValue = axis.AxisScaleTypeMap.ConvertValue(txtValue.EditValue, CultureInfo.CurrentCulture);
				if (axis.AxisScaleTypeMap.IsCompatible(nativeValue))
					return;
			}
			e.Cancel = true;
		}
		void txtValue_Validated(object sender, EventArgs e) {
			if (line == null)
				return;
			line.AxisValue = txtValue.EditValue;
			SetValidState();
		}
		void chCheckableInLegend_CheckedChanged(object sender, EventArgs e) {
			if (line == null)
				return;
			line.CheckableInLegend = chCheckableInLegend.Checked;
			UpdateControls();
		}
		void chCheckedInLegend_CheckedChanged(object sender, EventArgs e) {
			if (line == null)
				return;
			line.CheckedInLegend = chCheckedInLegend.Checked;
		}
		public void Initialize(ConstantLine line, AxisBase axis) {
			this.line = line;
			this.axis = axis;
			this.Enabled = line != null;
			if (line != null) {
				this.chLegendVisible.Checked = line.ShowInLegend;
				this.txtLegendText.EditValue = line.LegendText;
				this.txtName.EditValue = line.Name;
				this.chVisible.Checked = line.Visible;
				this.chShowBehind.Checked = line.ShowBehind;
				this.txtValue.EditValue = line.AxisValue;
				this.chCheckableInLegend.Checked = line.CheckableInLegend;
				this.chCheckedInLegend.Checked = line.CheckedInLegend;
				UpdateControls();
			}
			else {
				this.txtName.Text = "";
				this.txtLegendText.Text = "";
			}
		}
	}
}
