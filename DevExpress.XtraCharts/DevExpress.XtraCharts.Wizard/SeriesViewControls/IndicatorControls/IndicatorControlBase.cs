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
using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class IndicatorControlBase : ChartUserControl {
		Indicator indicator;
		Chart Chart {
			get {
				if (indicator == null)
					return null;
				else
					return ((IOwnedElement)indicator).Owner.Owner.Owner.Owner as Chart; }
		}
		protected Indicator Indicator { 
			get { return indicator; } 
		}
		public event MethodInvoker NameChanged;
		public IndicatorControlBase() {
			InitializeComponent();
		}
		void UpdateChecableAndCheckedInLegendEnabled() {
			if (Chart == null)
				return;
			chkCheckableInLegend.Enabled = indicator.Visible && indicator.ShowInLegend && Chart.Legend.UseCheckBoxes;
			chkCheckedInLegend.Enabled = chkCheckableInLegend.Enabled && indicator.CheckableInLegend;
		}
		void txtName_EditValueChanged(object sender, EventArgs e) {
			indicator.Name = txtName.Text;
			if (NameChanged != null)
				NameChanged();
		}
		void chkVisible_CheckedChanged(object sender, EventArgs e) {
			indicator.Visible = chkVisible.Checked;
			UpdateControls();
		}
		void chkShowInLegend_CheckedChanged(object sender, EventArgs e) {
			indicator.ShowInLegend = chkShowInLegend.Checked;
			UpdateChecableAndCheckedInLegendEnabled();
		}
		void chkCheckableInLegend_CheckedChanged(object sender, EventArgs e) {
			if (indicator == null)
				return;
			indicator.CheckableInLegend = chkCheckableInLegend.Checked;
			UpdateChecableAndCheckedInLegendEnabled();
		}
		void chkCheckedInLegend_CheckedChanged(object sender, EventArgs e) {
			if (indicator == null)
				return;
			indicator.CheckedInLegend = chkCheckedInLegend.Checked;
		}
		protected virtual void UpdateControls() {
			if (indicator == null)
				return;
			chkShowInLegend.Enabled = indicator.Visible;
		}
		public virtual void Initialize(Indicator indicator) {
			this.indicator = indicator;
			txtName.Text = indicator.Name;
			chkVisible.Checked = indicator.Visible;
			chkShowInLegend.Checked = indicator.ShowInLegend;
			chkCheckableInLegend.Checked = indicator.CheckableInLegend;
			chkCheckedInLegend.Checked = indicator.CheckedInLegend;
			UpdateChecableAndCheckedInLegendEnabled();
			UpdateControls();
		}
	}
}
