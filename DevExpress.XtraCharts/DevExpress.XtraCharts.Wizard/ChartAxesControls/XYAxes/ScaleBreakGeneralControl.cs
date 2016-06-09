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
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class ScaleBreakGeneralControl : ValidateControl {
		ScaleBreak scaleBreak;
		IAxisData axis;
		public event MethodInvoker UpdateByContent;
		public ScaleBreakGeneralControl() {
			InitializeComponent();
		}
		public void Initialize(ScaleBreak scaleBreak, AxisBase axis) {
			this.scaleBreak = scaleBreak;
			this.axis = axis;
			chVisible.Checked = scaleBreak.Visible;
			txtName.EditValue = scaleBreak.Name;
			txtEdge1.EditValue = scaleBreak.Edge1;
			txtEdge2.EditValue = scaleBreak.Edge2;
			pnlControls.Enabled = scaleBreak.Visible;
		}
		void OnUpdateByContent() {
			if(UpdateByContent != null)
				UpdateByContent();
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			scaleBreak.Visible = chVisible.Checked;
			pnlControls.Enabled = chVisible.Checked;
		}
		void txtName_Validating(object sender, CancelEventArgs e) {
			SetInvalidState();
			if (txtName.EditValue == null)
				e.Cancel = true;
		}
		void txtName_Validated(object sender, EventArgs e) {
			scaleBreak.Name = (string)txtName.EditValue;
			OnUpdateByContent();
			SetValidState();
		}
		void txtEdge1_Validating(object sender, CancelEventArgs e) {
			SetInvalidState();
			if (!String.IsNullOrEmpty((string)txtEdge1.Text)) {
				object nativeValue = axis.AxisScaleTypeMap.ConvertValue(txtEdge1.EditValue, CultureInfo.CurrentCulture);
				if (axis.AxisScaleTypeMap.IsCompatible(nativeValue)) {
					scaleBreak.Edge1 = nativeValue;
					return;
				}
			}
			e.Cancel = true;
		}
		void txtEdge1_Validated(object sender, EventArgs e) {
			scaleBreak.Edge1 = txtEdge1.EditValue;
			SetValidState();
		}
		void txtEdge2_Validating(object sender, CancelEventArgs e) {
			SetInvalidState();
			if (!String.IsNullOrEmpty((string)txtEdge2.Text)) {
				object nativeValue = axis.AxisScaleTypeMap.ConvertValue(txtEdge2.EditValue, CultureInfo.CurrentCulture);
				if (axis.AxisScaleTypeMap.IsCompatible(nativeValue)) {
					scaleBreak.Edge2 = nativeValue;
					return;
				}
			}
			e.Cancel = true;
		}
		void txtEdge2_Validated(object sender, EventArgs e) {
			scaleBreak.Edge2 = txtEdge2.EditValue;
			SetValidState();
		}
	}
}
