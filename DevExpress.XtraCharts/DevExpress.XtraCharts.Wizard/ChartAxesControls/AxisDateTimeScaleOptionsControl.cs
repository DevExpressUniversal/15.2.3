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
using DevExpress.XtraCharts.Wizard.ChartAxesControls;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class AxisDateTimeScaleOptionsControl : AxisScaleOptionsControlBase {
		struct DateTimeMeasureUnitItem {
			readonly DateTimeMeasureUnit measureUnit;
			readonly string text;
			public DateTimeMeasureUnit MeasureUnit { get { return measureUnit; } }
			public DateTimeMeasureUnitItem(DateTimeMeasureUnit measureUnit) {
				this.measureUnit = measureUnit;
				this.text = GetMeasureUnitText(measureUnit);
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is DateTimeMeasureUnitItem) && measureUnit == ((DateTimeMeasureUnitItem)obj).measureUnit;
			}
			public override int GetHashCode() {
				return measureUnit.GetHashCode();
			}
		}
		struct DateTimeGridAlignmentItem {
			readonly DateTimeGridAlignment gridAlignment;
			readonly string text;
			public DateTimeGridAlignment GridAlignment { get { return gridAlignment; } }
			public DateTimeGridAlignmentItem(DateTimeGridAlignment gridAlignment) {
				this.gridAlignment = gridAlignment;
				text = GetGridAlignmentText(gridAlignment);
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is DateTimeGridAlignmentItem) && (gridAlignment == ((DateTimeGridAlignmentItem)obj).gridAlignment);
			}
			public override int GetHashCode() {
				return gridAlignment.GetHashCode();
			}
		}
		static void FillDateTimeMeasureUnitComboBox(ComboBoxEdit comboBox) {
			comboBox.Properties.Items.Add(new DateTimeMeasureUnitItem(DateTimeMeasureUnit.Year));
			comboBox.Properties.Items.Add(new DateTimeMeasureUnitItem(DateTimeMeasureUnit.Quarter));
			comboBox.Properties.Items.Add(new DateTimeMeasureUnitItem(DateTimeMeasureUnit.Month));
			comboBox.Properties.Items.Add(new DateTimeMeasureUnitItem(DateTimeMeasureUnit.Week));
			comboBox.Properties.Items.Add(new DateTimeMeasureUnitItem(DateTimeMeasureUnit.Day));
			comboBox.Properties.Items.Add(new DateTimeMeasureUnitItem(DateTimeMeasureUnit.Hour));
			comboBox.Properties.Items.Add(new DateTimeMeasureUnitItem(DateTimeMeasureUnit.Minute));
			comboBox.Properties.Items.Add(new DateTimeMeasureUnitItem(DateTimeMeasureUnit.Second));
			comboBox.Properties.Items.Add(new DateTimeMeasureUnitItem(DateTimeMeasureUnit.Millisecond));
		}
		static void FillGridAlignmentComboBox(ComboBoxEdit comboBox) {
			comboBox.Properties.Items.Add(new DateTimeGridAlignmentItem(DateTimeGridAlignment.Year));
			comboBox.Properties.Items.Add(new DateTimeGridAlignmentItem(DateTimeGridAlignment.Quarter));
			comboBox.Properties.Items.Add(new DateTimeGridAlignmentItem(DateTimeGridAlignment.Month));
			comboBox.Properties.Items.Add(new DateTimeGridAlignmentItem(DateTimeGridAlignment.Week));
			comboBox.Properties.Items.Add(new DateTimeGridAlignmentItem(DateTimeGridAlignment.Day));
			comboBox.Properties.Items.Add(new DateTimeGridAlignmentItem(DateTimeGridAlignment.Hour));
			comboBox.Properties.Items.Add(new DateTimeGridAlignmentItem(DateTimeGridAlignment.Minute));
			comboBox.Properties.Items.Add(new DateTimeGridAlignmentItem(DateTimeGridAlignment.Second));
			comboBox.Properties.Items.Add(new DateTimeGridAlignmentItem(DateTimeGridAlignment.Millisecond));
		}
		static string GetMeasureUnitText(DateTimeMeasureUnit measureUnit) {
			switch (measureUnit) {
				case DateTimeMeasureUnit.Year:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitYear);
				case DateTimeMeasureUnit.Quarter:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitQuarter);
				case DateTimeMeasureUnit.Month:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitMonth);
				case DateTimeMeasureUnit.Week:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitWeek);
				case DateTimeMeasureUnit.Day:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitDay);
				case DateTimeMeasureUnit.Hour:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitHour);
				case DateTimeMeasureUnit.Minute:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitMinute);
				case DateTimeMeasureUnit.Second:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitSecond);
				case DateTimeMeasureUnit.Millisecond:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitMillisecond);
				default:
					ChartDebug.Fail("Unknown DateTime measure unit.");
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitMonth);
			}
		}
		static string GetGridAlignmentText(DateTimeGridAlignment gridAlignment) {
			switch (gridAlignment) {
				case DateTimeGridAlignment.Year:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitYear);
				case DateTimeGridAlignment.Quarter:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitQuarter);
				case DateTimeGridAlignment.Month:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitMonth);
				case DateTimeGridAlignment.Week:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitWeek);
				case DateTimeGridAlignment.Day:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitDay);
				case DateTimeGridAlignment.Hour:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitHour);
				case DateTimeGridAlignment.Minute:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitMinute);
				case DateTimeGridAlignment.Second:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitSecond);
				case DateTimeGridAlignment.Millisecond:
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitMillisecond);
				default:
					ChartDebug.Fail("Unknown DateTime grid alignment unit.");
					return ChartLocalizer.GetString(ChartStringId.WizDateTimeMeasureUnitMonth);
			}
		}
		DateTimeScaleOptions DateTimeOptions { get { return (DateTimeScaleOptions)base.ScaleOptions; } }
		public AxisDateTimeScaleOptionsControl() {
			InitializeComponent();
			InitializeControls();
		}
		public void Initialize(AxisGeneralTabsControl generalControl, AxisBase axis, Chart chart) {
			base.Initialize(generalControl, axis.DateTimeScaleOptions, axis, chart);
		}
		void cmbMeasureUnit_SelectedIndexChanged(object sender, EventArgs e) {
			if (!Locked) {
				DateTimeOptions.MeasureUnit = ((DateTimeMeasureUnitItem)cmbMeasureUnit.SelectedItem).MeasureUnit;
				UpdateControls();
			}
		}
		void cmbGridAlignment_SelectedIndexChanged(object sender, EventArgs e) {
			cmbGridAlignment.DoValidate();
		}
		void cmbGridAlignment_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if (!Locked) {
				SetInvalidState();
				try {
					DateTimeOptions.GridAlignment = ((DateTimeGridAlignmentItem)cmbGridAlignment.SelectedItem).GridAlignment;
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
		protected override void CollectActiveControlsCore(IList<Control> activeControls) {
			if (SupportsScaleMode) {
				activeControls.Add(pnlMeasureUnitsSeparator);
				activeControls.Add(pnlMeasureUnit);
				activeControls.Add(pnlInnerSeparator);
			}
		}
		protected override void CollectActiveGridControlsCore(IList<Control> activeControls) {
			activeControls.Add(pnlGridAlignment);
		}
		protected override void UpdateControlsCore() {
			base.UpdateControlsCore();
			pnlMeasureUnit.Enabled = SupportsMeasureUnits;
			pnlGridAlignment.Enabled = SupportsGridAlignment;
			cmbMeasureUnit.SelectedItem = new DateTimeMeasureUnitItem(DateTimeOptions.MeasureUnit);
			cmbGridAlignment.SelectedItem = new DateTimeGridAlignmentItem(DateTimeOptions.GridAlignment);
		}
		protected override void InitializeControlsCore() {
			base.InitializeControlsCore();
			FillDateTimeMeasureUnitComboBox(cmbMeasureUnit);
			FillGridAlignmentComboBox(cmbGridAlignment);
		}
		protected override string GetGridAlignmentLabelText() {
			return ", " + GetGridAlignmentText(DateTimeOptions.GridAlignment);	  
		}
		protected override string GetMeasureUnitLabelText() {
			return ", " + GetMeasureUnitText(DateTimeOptions.MeasureUnit);
		}
	}
}
