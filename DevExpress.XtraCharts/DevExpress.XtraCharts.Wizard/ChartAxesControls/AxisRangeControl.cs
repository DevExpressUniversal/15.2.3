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
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class AxisRangeControl : ValidateControl {
		AxisBase axis;
		Range range;
		AxisGeneralTabsControl generalControl;
		int lockCounter = 0;
		bool Locked { 
			get {
				ChartDebug.Assert(lockCounter >= 0, "Invalid lock state");
				return lockCounter > 0;
			} 
		}
		VisualRange VisualRange { get { return range as VisualRange; } }
		WholeRange WholeRange { get { return range as WholeRange; } }
		IAxisData AxisData { get { return axis as IAxisData; } }
		bool IsRadarAxisX { get { return axis is RadarAxisX; } }
		bool IsPolarAxisX { get { return axis is PolarAxisX; } }
		bool IsAxisY { get { return axis is AxisYBase || axis is AxisY3D || axis is RadarAxisY; } }
		bool IsNumerical { get { return AxisData.AxisScaleTypeMap.ScaleType == ActualScaleType.Numerical; } }
		bool SideMarginsSupported { get { return !IsRadarAxisX; } }
		bool ShowZeroLevelSupported { get { return IsNumerical && IsAxisY && WholeRange != null; } }
		bool AlwaysShowZeroLevel {
			get { return WholeRange != null ? WholeRange.AlwaysShowZeroLevel : false; }
			set {
				if (WholeRange != null)
					WholeRange.AlwaysShowZeroLevel = value;
			}
		}
		public AxisRangeControl() {
			InitializeComponent();
		}
		void Lock() {
			lockCounter++;
		}
		void Unlock() {
			lockCounter--;
		}
		void chAuto_CheckedChanged(object sender, EventArgs e) {
			if (!Locked) {
				range.Auto = chAuto.Checked;
				generalControl.UpdateRanges();
			}
		}
		void chAutoSideMargins_CheckedChanged(object sender, EventArgs e) {
			if (!Locked) {
				range.AutoSideMargins = chAutoSideMargins.Checked;
				generalControl.UpdateRanges();
			}
		}
		void txtMinValue_Validating(object sender, CancelEventArgs e) {
			if (!Locked) {
				e.Cancel = !ValidateMinValue(txtMinValue.EditValue);
				SetInvalidState();
			}
		}
		void txtMinValue_Validated(object sender, EventArgs e) {
			if (!Locked) {
				SetMinValue(txtMinValue.EditValue);
				generalControl.UpdateRanges();
				SetValidState();
			}
		}
		void txtMaxValue_Validating(object sender, CancelEventArgs e) {
			if (!Locked) {
				e.Cancel = !ValidateMaxValue(txtMaxValue.EditValue);
				SetInvalidState();
			}
		}
		void txtMaxValue_Validated(object sender, EventArgs e) {
			if (!Locked) {
				SetMaxValue(txtMaxValue.EditValue);
				generalControl.UpdateRanges();
				SetValidState();
			}
		}
		void chShowZeroLevel_CheckedChanged(object sender, EventArgs e) {
			if (!Locked) {
				AlwaysShowZeroLevel = chShowZeroLevel.Checked;
				UpdateControls();
			}
		}
		void txtSideMarginsValue_EditValueChanged(object sender, EventArgs e) {
			if (!Locked) {
				range.SideMarginsValue = Convert.ToDouble(txtSideMarginsValue.EditValue);
				generalControl.UpdateRanges();
			}
		}
		void SetMinValue(object minValue) {
			if (minValue != null && !minValue.Equals(range.MinValue))
				try {
					range.MinValue = AxisData.AxisScaleTypeMap.ConvertValue(minValue, CultureInfo.CurrentCulture);
				}
				catch {
				}
		}
		void SetMaxValue(object maxValue) {
			if (maxValue != null && !maxValue.Equals(range.MaxValue))
				try {
					range.MaxValue = AxisData.AxisScaleTypeMap.ConvertValue(maxValue, CultureInfo.CurrentCulture);
				}
				catch {
				}
		}
		bool ValidateMinValue(object minValue) {
			try {
				object nativeValue = AxisData.AxisScaleTypeMap.ConvertValue(minValue, CultureInfo.CurrentCulture);
				return AxisData.AxisScaleTypeMap.IsCompatible(nativeValue);
			}
			catch {
			}
			return false;
		}
		bool ValidateMaxValue(object maxValue) {
			try {
				object nativeValue = AxisData.AxisScaleTypeMap.ConvertValue(maxValue, CultureInfo.CurrentCulture);
				return AxisData.AxisScaleTypeMap.IsCompatible(nativeValue);
			}
			catch {
			}
			return false;
		}
		IList<Control> GetActiveControls() {
			if (IsPolarAxisX)
				return new Control[0];
			List<Control> activeControls = new List<Control>();
			if (SideMarginsSupported) {
				activeControls.Add(grpSideMargins);
				activeControls.Add(sepSideMargins);
			}
			if (ShowZeroLevelSupported) {
				activeControls.Add(pnlShowZeroLevel);
				activeControls.Add(sepShowZeroLevel);
			}
			activeControls.Add(pnlMaxValue);
			activeControls.Add(sepMaxValue);
			activeControls.Add(pnlMinValue);
			activeControls.Add(sepMinValue);
			activeControls.Add(lblMinValueLine);
			activeControls.Add(pnlAuto);
			return activeControls;
		}
		public void UpdateControls() {
			Lock();
			try {
				chAuto.Checked = range.Auto;
				txtMinValue.EditValue = range.MinValue;
				pnlMinValue.Enabled = !range.Auto;
				txtMaxValue.EditValue = range.MaxValue;
				pnlMaxValue.Enabled = !range.Auto;
				grpSideMargins.Enabled = !range.Auto || WholeRange != null;
				chShowZeroLevel.Checked = AlwaysShowZeroLevel;
				pnlShowZeroLevel.Enabled = range.Auto;
				chAutoSideMargins.Checked = range.AutoSideMargins;
				pnlSideMarginsValue.Enabled = !range.AutoSideMargins;
				txtSideMarginsValue.EditValue = range.SideMarginsValue;
			}
			finally {
				Unlock();
			}
		}
		public bool Initialize(AxisBase axis, Range range, AxisGeneralTabsControl generalControl) {
			this.axis = axis;
			this.range = range;
			this.generalControl = generalControl;
			Hide();
			try {
				Controls.Clear();
				IList<Control> actualControls = GetActiveControls();
				if (actualControls.Count <= 0)
					return false;
				foreach (Control control in actualControls) {
					Controls.Add(control);
					control.SendToBack();
				}
				UpdateControls();
			}
			finally {
				Show();
			}
			return true;
		}
	}
}
