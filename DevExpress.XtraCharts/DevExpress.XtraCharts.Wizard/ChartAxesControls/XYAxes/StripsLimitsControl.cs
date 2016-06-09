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
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class StripsLimitsControl : ChartUserControl {
		Strip strip;
		Axis2D axis;
		IAxisData Axis { get { return axis; } } 
		bool Locked { get { return strip == null; } }
		public StripsLimitsControl() {
			InitializeComponent();
		}
		public void Initialize(Strip strip, Axis2D axis) {
			this.strip = strip;
			this.Enabled = strip != null;
			this.axis = axis;
			if (strip != null) {
				this.chMinEnabled.Checked = strip.MinLimit.Enabled;
				this.chMaxEnabled.Checked = strip.MaxLimit.Enabled;
				UpdateControls();
			}
			else {
				this.chMinEnabled.Checked = false;
				this.txtMinValue.EditValue = "";
				this.chMaxEnabled.Checked = false;
				this.txtMaxValue.EditValue = "";
			}
		}
		void UpdateControls() {
			pnlMinValue.Enabled = strip.MinLimit.Enabled;
			pnlMaxValue.Enabled = strip.MaxLimit.Enabled;
			txtMinValue.EditValue = strip.MinLimit.AxisValue;
			txtMaxValue.EditValue = strip.MaxLimit.AxisValue;
		}
		void chMinVisible_CheckedChanged(object sender, EventArgs e) {
			if (!Locked) {
				strip.MinLimit.Enabled = chMinEnabled.Checked;
				UpdateControls();
			}
		}
		void chMaxVivible_CheckedChanged(object sender, EventArgs e) {
			if (!Locked) {
				strip.MaxLimit.Enabled = chMaxEnabled.Checked;
				UpdateControls();
			}
		}
		void txtMinValue_Validating(object sender, CancelEventArgs e) {
			if (!Locked) {
				object nativeValue = Axis.AxisScaleTypeMap.ConvertValue(txtMinValue.Text, CultureInfo.CurrentCulture);
				e.Cancel = !Axis.AxisScaleTypeMap.IsCompatible(nativeValue) || (strip.MaxLimit.Enabled && !StripLimitsUtils.CheckLimits(axis, nativeValue, strip.MaxLimit.AxisValue));
			}			
		}
		void txtMaxValue_Validating(object sender, CancelEventArgs e) {
			if (!Locked) {
				object nativeValue = Axis.AxisScaleTypeMap.ConvertValue(txtMaxValue.Text, CultureInfo.CurrentCulture);
				e.Cancel = !Axis.AxisScaleTypeMap.IsCompatible(nativeValue) || (strip.MinLimit.Enabled && !StripLimitsUtils.CheckLimits(axis, strip.MinLimit.AxisValue, nativeValue));
			}			
		}
		void txtMinValue_Validated(object sender, EventArgs e) {
			if (!Locked)
				strip.MinLimit.AxisValue = txtMinValue.EditValue;
		}
		void txtMaxValue_Validated(object sender, EventArgs e) {
			if (!Locked)
				strip.MaxLimit.AxisValue = txtMaxValue.EditValue;
		}
	}
}
