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
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class AxisNumericScaleOptionsControl : AxisScaleOptionsControlBase {
		struct NumericMeasureUnitItem {
			readonly NumericMeasureUnit measureUnit;
			readonly string text;
			public NumericMeasureUnit MeasureUnit { get { return measureUnit; } }
			public NumericMeasureUnitItem(NumericMeasureUnit measureUnit) {
				this.measureUnit = measureUnit;
				this.text = GetMeasureUnitText(measureUnit);
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is NumericMeasureUnitItem) && measureUnit == ((NumericMeasureUnitItem)obj).measureUnit;
			}
			public override int GetHashCode() {
				return measureUnit.GetHashCode();
			}
		}
		struct NumericGridAlignmentItem {
			readonly NumericGridAlignment gridAlignment;
			readonly string text;
			public NumericGridAlignment GridAlignment { get { return gridAlignment; } }
			public NumericGridAlignmentItem(NumericGridAlignment gridAlignment) {
				this.gridAlignment = gridAlignment;
				text = GetGridAlignmentText(gridAlignment);
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is NumericGridAlignmentItem) && (gridAlignment == ((NumericGridAlignmentItem)obj).gridAlignment);
			}
			public override int GetHashCode() {
				return gridAlignment.GetHashCode();
			}
		}
		static void FillNumericMeasureUnitComboBox(ComboBoxEdit comboBox) {
			comboBox.Properties.Items.Add(new NumericMeasureUnitItem(NumericMeasureUnit.Custom));
			comboBox.Properties.Items.Add(new NumericMeasureUnitItem(NumericMeasureUnit.Billions));
			comboBox.Properties.Items.Add(new NumericMeasureUnitItem(NumericMeasureUnit.Millions));
			comboBox.Properties.Items.Add(new NumericMeasureUnitItem(NumericMeasureUnit.Thousands));
			comboBox.Properties.Items.Add(new NumericMeasureUnitItem(NumericMeasureUnit.Hundreds));
			comboBox.Properties.Items.Add(new NumericMeasureUnitItem(NumericMeasureUnit.Tens));
			comboBox.Properties.Items.Add(new NumericMeasureUnitItem(NumericMeasureUnit.Ones));
		}
		static void FillNumericGridAlignmentComboBox(ComboBoxEdit comboBox) {
			comboBox.Properties.Items.Add(new NumericGridAlignmentItem(NumericGridAlignment.Custom));
			comboBox.Properties.Items.Add(new NumericGridAlignmentItem(NumericGridAlignment.Billions));
			comboBox.Properties.Items.Add(new NumericGridAlignmentItem(NumericGridAlignment.Millions));
			comboBox.Properties.Items.Add(new NumericGridAlignmentItem(NumericGridAlignment.Thousands));
			comboBox.Properties.Items.Add(new NumericGridAlignmentItem(NumericGridAlignment.Hundreds));
			comboBox.Properties.Items.Add(new NumericGridAlignmentItem(NumericGridAlignment.Tens));
			comboBox.Properties.Items.Add(new NumericGridAlignmentItem(NumericGridAlignment.Ones));
		}
		static string GetGridAlignmentText(NumericGridAlignment gridAlignment) {
			switch (gridAlignment) {
				case NumericGridAlignment.Billions:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitBillions);
				case NumericGridAlignment.Custom:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitCustom);
				case NumericGridAlignment.Hundreds:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitHundreds);
				case NumericGridAlignment.Millions:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitMillions);
				case NumericGridAlignment.Tens:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitTens);
				case NumericGridAlignment.Ones:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitOnes);
				case NumericGridAlignment.Thousands:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitThousands);
				default:
					ChartDebug.Fail("Unknown numeric grid alignment.");
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitTens);
			}
		}
		static string GetMeasureUnitText(NumericMeasureUnit measureUnit) {
			switch (measureUnit) {
				case NumericMeasureUnit.Billions:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitBillions);
				case NumericMeasureUnit.Custom:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitCustom);
				case NumericMeasureUnit.Hundreds:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitHundreds);
				case NumericMeasureUnit.Millions:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitMillions);
				case NumericMeasureUnit.Tens:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitTens);
				case NumericMeasureUnit.Ones:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitOnes);
				case NumericMeasureUnit.Thousands:
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitThousands);
				default:
					ChartDebug.Fail("Unknown numeric measure unit.");
					return ChartLocalizer.GetString(ChartStringId.WizNumericMeasureUnitTens);
			}
		}
		NumericScaleOptions NumericOptions { get { return (NumericScaleOptions)base.ScaleOptions; } }
		public AxisNumericScaleOptionsControl() {
			InitializeComponent();
			InitializeControls();
		}
		public void Initialize(AxisGeneralTabsControl generalControl, AxisBase axis, Chart chart) {
			base.Initialize(generalControl, axis.NumericScaleOptions, axis, chart);
		}
		void cmbMeasureUnit_SelectedIndexChanged(object sender, EventArgs e) {
			if (!Locked) {
				NumericOptions.MeasureUnit = ((NumericMeasureUnitItem)cmbMeasureUnit.SelectedItem).MeasureUnit;
				UpdateControls();
			}
		}
		void cmbGridAlignment_SelectedIndexChanged(object sender, EventArgs e) {
			cmbGridAlignment.DoValidate();
		}
		void spnCustomMeasureUnit_EditValueChanged(object sender, EventArgs e) {
			if (!Locked) {
				NumericOptions.CustomMeasureUnit = Convert.ToDouble(spnCustomMeasureUnit.EditValue);
				UpdateControls();
			}
		}
		void cmbGridAlignment_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if (!Locked) {
				SetInvalidState();
				try {
					NumericOptions.GridAlignment = ((NumericGridAlignmentItem)cmbGridAlignment.SelectedItem).GridAlignment;
				}
				catch (Exception ex) {
					e.Cancel = true;
					cmbGridAlignment.ErrorText = ex.Message;
				}
			}
		}
		void cmbGridAlignment_Validated(object sender, EventArgs e) {
			if (!Locked) {
				SetValidState();
				UpdateControls();
			}
		}
		void spnCustomGridAlignment_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if (!Locked) {
				SetInvalidState();
				try {
					NumericOptions.CustomGridAlignment = Convert.ToDouble(spnCustomGridAlignment.EditValue);
				}
				catch (Exception ex) {
					e.Cancel = true;
					spnCustomGridAlignment.ErrorText = ex.Message;
				}
			}
		}
		void spnCustomGridAlignment_Validated(object sender, EventArgs e) {
			if (!Locked) {
				SetValidState();
				UpdateControls();
			}
		}
		protected override void CollectActiveControlsCore(IList<Control> activeControls) {
			if (SupportsScaleMode) {
				activeControls.Add(pnlCustomMeasureUnitSeparator);
				activeControls.Add(pnlCustomMeasureUnit);
				activeControls.Add(pnlMeasureUnitsSeparator);
				activeControls.Add(pnlMeasureUnit);
				activeControls.Add(pnlInnerSeparator);
			}
		}
		protected override void CollectActiveGridControlsCore(IList<Control> activeControls) {
			activeControls.Add(pnlCustomGridAlignment);
			activeControls.Add(pnlGridAlignmentSeparator);
			activeControls.Add(pnlGridAlignment);
		}
		protected override void UpdateControlsCore() {
			base.UpdateControlsCore();
			pnlMeasureUnit.Enabled = SupportsMeasureUnits;
			pnlGridAlignment.Enabled = SupportsGridAlignment;
			pnlCustomGridAlignment.Enabled = SupportsGridAlignment && NumericOptions.GridAlignment == NumericGridAlignment.Custom;
			pnlCustomMeasureUnit.Enabled = SupportsMeasureUnits && NumericOptions.MeasureUnit == NumericMeasureUnit.Custom;
			spnCustomGridAlignment.EditValue = NumericOptions.CustomGridAlignment;
			spnCustomMeasureUnit.EditValue = NumericOptions.CustomMeasureUnit;
			cmbMeasureUnit.SelectedItem = new NumericMeasureUnitItem(NumericOptions.MeasureUnit);
			cmbGridAlignment.SelectedItem = new NumericGridAlignmentItem(NumericOptions.GridAlignment);
		}
		protected override void InitializeControlsCore() {
			base.InitializeControlsCore();
			FillNumericGridAlignmentComboBox(cmbGridAlignment);
			FillNumericMeasureUnitComboBox(cmbMeasureUnit);
			spnGridSpacing.Properties.MaxValue = 1000000;
		}
		protected override string GetGridAlignmentLabelText() {
			if (NumericOptions.GridAlignment != NumericGridAlignment.Custom)
				return ", " + GetGridAlignmentText(NumericOptions.GridAlignment);
			return base.GetGridAlignmentLabelText();
		}
		protected override string GetMeasureUnitLabelText() {
			if (NumericOptions.MeasureUnit != NumericMeasureUnit.Custom)
				return ", " + GetMeasureUnitText(NumericOptions.MeasureUnit);
			return base.GetMeasureUnitLabelText();
		}
	}
}
