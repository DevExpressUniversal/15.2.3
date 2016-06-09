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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class TickmarksControl : ChartUserControl {
		TickmarksBase tickmarks;
		AxisBase axis;
		public TickmarksControl() {
			InitializeComponent();
		}
		public void Initialize(TickmarksBase tickmarks, AxisBase axis) {
			this.tickmarks = tickmarks;
			this.axis = axis;
			chVisible.Checked = tickmarks.Visible;
			chCrossAxis.Checked = tickmarks.CrossAxis;
			txtLength.EditValue = tickmarks.Length;
			txtThickness.EditValue = tickmarks.Thickness;
			chMinorVisible.Checked = tickmarks.MinorVisible;
			txtMinorLength.EditValue = tickmarks.MinorLength;
			txtMinorThickness.EditValue = tickmarks.MinorThickness;
			InitializeMinorCount();
		}
		public void InitializeMinorCount() {
			if (tickmarks != null) {
				if (axis != null)
					spnMinorCount.EditValue = axis.MinorCount;
				else
					ChartDebug.Fail("The axis can't be null.");
			}
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			tickmarks.Visible = chVisible.Checked;
			UpdateControls();
		}
		void UpdateControls() {
			chCrossAxis.Enabled = tickmarks.Visible || tickmarks.MinorVisible;
			pnlMajorControls.Enabled = tickmarks.Visible;
			pnlMinorControls.Enabled = tickmarks.MinorVisible;
		}
		void chCrossAxis_CheckedChanged(object sender, EventArgs e) {
			tickmarks.CrossAxis = chCrossAxis.Checked;
		}
		void txtLength_EditValueChanged(object sender, EventArgs e) {
			tickmarks.Length = Convert.ToInt32(txtLength.EditValue);
		}
		void txtThickness_EditValueChanged(object sender, EventArgs e) {
			tickmarks.Thickness = Convert.ToInt32(txtThickness.EditValue);
		}
		void chMinorVisible_CheckedChanged(object sender, EventArgs e) {
			tickmarks.MinorVisible = chMinorVisible.Checked;
			UpdateControls();
		}
		void txtMinorLength_EditValueChanged(object sender, EventArgs e) {
			tickmarks.MinorLength = Convert.ToInt32(txtMinorLength.EditValue);
		}
		void txtMinorThickness_EditValueChanged(object sender, EventArgs e) {
			tickmarks.MinorThickness = Convert.ToInt32(txtMinorThickness.EditValue);
		}
	   void spnMinorCount_EditValueChanged(object sender, EventArgs e) {
			if (axis != null)
				axis.MinorCount = Convert.ToInt32(spnMinorCount.EditValue);
		}
	}
}
