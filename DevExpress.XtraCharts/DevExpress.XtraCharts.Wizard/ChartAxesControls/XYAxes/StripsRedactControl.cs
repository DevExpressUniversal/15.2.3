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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class StripsRedactControl : ValidateControl {
		Strip strip;
		Chart Chart {
			get {
				if (strip == null)
					return null;
				else
					return ((IOwnedElement)strip).Owner.Owner.Owner as Chart;
			}
		}
		public event MethodInvoker UpdateByContent;
		public StripsRedactControl() {
			InitializeComponent();
		}
		void OnUpdateByContent() {
			if (UpdateByContent != null)
				UpdateByContent();
		}
		void UpdateControls() {
			pnlName.Enabled = strip.Visible;
			pnlLabelOptions.Enabled = strip.Visible;
			pnlLabelControls.Enabled = strip.ShowAxisLabel;
			pnlLegendControls.Enabled = strip.ShowInLegend;
			chCheckableInLegend.Enabled = strip.Visible && strip.ShowInLegend && Chart.Legend.UseCheckBoxes;
			chCheckedInLegend.Enabled = chCheckableInLegend.Enabled && strip.CheckableInLegend;
			txtLegendText.Enabled = strip.Visible && strip.ShowInLegend;
			lblLegendText.Enabled = strip.Visible && strip.ShowInLegend;
		}
		void chLabelVisible_CheckedChanged(object sender, EventArgs e) {
			if (strip == null)
				return;
			this.strip.ShowAxisLabel = this.chLabelVisible.Checked;
			UpdateControls();
		}
		void chCheckableInLegend_CheckedChanged(object sender, EventArgs e) {
			if (strip == null)
				return;
			strip.CheckableInLegend = chCheckableInLegend.Checked;
			UpdateControls();
		}
		void chCheckedInLegend_CheckedChanged(object sender, EventArgs e) {
			if (strip == null)
				return;
			strip.CheckedInLegend = chCheckedInLegend.Checked;
		}
		void txtLabelText_EditValueChanged(object sender, EventArgs e) {
			if (strip == null)
				return;
			this.strip.AxisLabelText = this.txtLabelText.EditValue.ToString();
		}
		void chLegendVisible_CheckedChanged(object sender, EventArgs e) {
			if (strip == null)
				return;
			this.strip.ShowInLegend = this.chLegendVisible.Checked;
			UpdateControls();
		}
		void txtLegendText_EditValueChanged(object sender, EventArgs e) {
			if (strip == null)
				return;
			this.strip.LegendText = this.txtLegendText.EditValue.ToString();
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			if (strip == null)
				return;
			this.strip.Visible = this.chVisible.Checked;
			UpdateControls();
		}
		void txtName_Validating(object sender, CancelEventArgs e) {
			if (this.txtName.EditValue != null)
				return;
			e.Cancel = true;
			SetInvalidState();
		}
		void txtName_Validated(object sender, EventArgs e) {
			if (strip == null)
				return;
			this.strip.Name = this.txtName.EditValue.ToString();
			OnUpdateByContent();
			SetValidState();
		}
		public void Initialize(Strip strip) {
			this.strip = strip;
			this.Enabled = strip != null;
			if (strip != null) {
				this.chLabelVisible.Checked = strip.ShowAxisLabel;
				this.chLegendVisible.Checked = strip.ShowInLegend;
				this.txtLabelText.EditValue = strip.AxisLabelText;
				this.txtLegendText.EditValue = strip.LegendText;
				this.txtName.EditValue = strip.Name;
				this.chVisible.Checked = strip.Visible;
				this.chCheckableInLegend.Checked = strip.CheckableInLegend;
				this.chCheckedInLegend.Checked = strip.CheckedInLegend;
				UpdateControls();
			}
			else {
				this.txtName.Text = "";
				this.txtLabelText.Text = "";
				this.txtLegendText.Text = "";
			}
		}
	}
}
