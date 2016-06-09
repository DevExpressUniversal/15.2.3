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
	internal partial class AxisScaleOptionsControlBase : ValidateControl {
		struct ScaleModeItem {
			readonly ScaleMode scaleMode;
			readonly string text;
			public ScaleMode ScaleMode { get { return scaleMode; } }
			public ScaleModeItem(ScaleMode scaleMode) {
				this.scaleMode = scaleMode;
				switch (scaleMode) {
					case ScaleMode.Manual:
						text = ChartLocalizer.GetString(ChartStringId.WizScaleModeManual);
						break;
					case ScaleMode.Automatic:
						text = ChartLocalizer.GetString(ChartStringId.WizScaleModeAutomatic);
						break;
					case ScaleMode.Continuous:
						text = ChartLocalizer.GetString(ChartStringId.WizScaleModeContinuous);
						break;
					default:
						ChartDebug.Fail("Unknown DateTime scale mode.");
						text = ChartLocalizer.GetString(ChartStringId.WizScaleModeManual);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is ScaleModeItem) && scaleMode == ((ScaleModeItem)obj).scaleMode;
			}
			public override int GetHashCode() {
				return scaleMode.GetHashCode();
			}
		}
		struct AggregateFunctionItem {
			readonly AggregateFunction function;
			readonly string text;
			public AggregateFunction Function { get { return function; } }
			public AggregateFunctionItem(AggregateFunction function) {
				this.function = function;
				switch (function) {
					case AggregateFunction.None:
						text = ChartLocalizer.GetString(ChartStringId.WizAggregateFunctionNone);
						break;
					case AggregateFunction.Sum:
						text = ChartLocalizer.GetString(ChartStringId.WizAggregateFunctionSum);
						break;
					case AggregateFunction.Average:
						text = ChartLocalizer.GetString(ChartStringId.WizAggregateFunctionAverage);
						break;
					case AggregateFunction.Financial:
						text = ChartLocalizer.GetString(ChartStringId.WizAggregateFunctionFinancial);
						break;
					case AggregateFunction.Count:
						text = ChartLocalizer.GetString(ChartStringId.WizAggregateFunctionCount);
						break;
					case AggregateFunction.Maximum:
						text = ChartLocalizer.GetString(ChartStringId.WizAggregateFunctionMaximum);
						break;
					case AggregateFunction.Minimum:
						text = ChartLocalizer.GetString(ChartStringId.WizAggregateFunctionMinimum);
						break;
					default:
						ChartDebug.Fail("Unknown AggregateFunction unit.");
						text = ChartLocalizer.GetString(ChartStringId.WizAggregateFunctionAverage);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is AggregateFunctionItem) && (function == ((AggregateFunctionItem)obj).function);
			}
			public override int GetHashCode() {
				return function.GetHashCode();
			}
		}
		static void FillAggregateFunctionsComboBox(ComboBoxEdit comboBox, bool isFinancial) {
			comboBox.Properties.Items.Clear();
			comboBox.Properties.Items.Add(new AggregateFunctionItem(AggregateFunction.None));
			comboBox.Properties.Items.Add(new AggregateFunctionItem(AggregateFunction.Average));
			comboBox.Properties.Items.Add(new AggregateFunctionItem(AggregateFunction.Sum));
			comboBox.Properties.Items.Add(new AggregateFunctionItem(AggregateFunction.Minimum));
			comboBox.Properties.Items.Add(new AggregateFunctionItem(AggregateFunction.Maximum));
			comboBox.Properties.Items.Add(new AggregateFunctionItem(AggregateFunction.Count));
			if (isFinancial)
				comboBox.Properties.Items.Add(new AggregateFunctionItem(AggregateFunction.Financial));
		}
		static void FillScaleModeComboBox(ComboBoxEdit comboBox) {
			comboBox.Properties.Items.Add(new ScaleModeItem(ScaleMode.Continuous));
			comboBox.Properties.Items.Add(new ScaleModeItem(ScaleMode.Automatic));
			comboBox.Properties.Items.Add(new ScaleModeItem(ScaleMode.Manual));
		}
		ScaleOptionsBase scaleOptions;
		AxisBase axis;
		AxisGeneralTabsControl generalControl;
		Chart chart;
		int lockCounter;
		readonly string spacingLabelText;
		readonly string offsetLabelText;
		bool UsePivotGridOptions { get { return PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(chart.DataContainer.PivotGridDataSourceOptions, Axis, false); } }
		bool SupportsGridSpacing { get { return !(axis is PolarAxisX); } }
		protected bool Locked {
			get {
				ChartDebug.Assert(lockCounter >= 0, "Invalid lock state.");
				return lockCounter > 0;
			}
		}
		protected bool SupportsScaleMode { get { return ((IAxisData)axis).IsArgumentAxis; } }
		protected bool SupportsMeasureUnits { get { return !UsePivotGridOptions && scaleOptions.ScaleMode == ScaleMode.Manual; } }
		protected bool SupportsGridAlignment { get { return !UsePivotGridOptions && !scaleOptions.AutoGrid; } }
		protected AxisBase Axis { get { return axis; } }
		protected ScaleOptionsBase ScaleOptions { get { return scaleOptions; } }
		public AxisScaleOptionsControlBase() {
			InitializeComponent();
			this.spacingLabelText = lblGridSpacing.Text;
			this.offsetLabelText = lblGridOffset.Text;
		}
		protected void Initialize(AxisGeneralTabsControl generalControl, ScaleOptionsBase scaleOptions, AxisBase axis, Chart chart) {
			this.scaleOptions = scaleOptions;
			this.axis = axis;
			this.generalControl = generalControl;
			this.chart = chart;
			FillAggregateFunctionsComboBox(cmbAggregateFunction, ChartContainsFinancialSeries());
			FillControls(this, CollectActiveControls);
			FillControls(grpGrid, CollectActiveGridControls);
			UpdateControls();
		}
		protected void Lock() {
			lockCounter++;
		}
		protected void Unlock() {
			lockCounter--;
		}
		protected void UpdateControls() {
			Lock();
			try {
				UpdateControlsCore();
			}
			finally {
				Unlock();
			}
		}
		protected virtual void CollectActiveControlsCore(IList<Control> activeControls) {
		}
		protected virtual void CollectActiveGridControlsCore(IList<Control> activeControls) {
		}
		protected virtual void UpdateControlsCore() {
			if (SupportsScaleMode) {
				cmbScaleMode.SelectedItem = new ScaleModeItem(scaleOptions.ScaleMode);
				pnlAggregateFunction.Enabled = (scaleOptions.ScaleMode != ScaleMode.Continuous);
			}
			cmbAggregateFunction.SelectedItem = new AggregateFunctionItem(scaleOptions.AggregateFunction);
			if (SupportsGridSpacing)
				UpdateGrid();
		}
		protected virtual void InitializeControlsCore() {
			FillScaleModeComboBox(cmbScaleMode);
		}
		protected virtual string GetMeasureUnitLabelText() {
			return String.Empty;
		}
		protected virtual string GetGridAlignmentLabelText() {
			return String.Empty;
		}
		protected void InitializeControls() {
			Lock();
			try {
				InitializeControlsCore();
			}
			finally {
				Unlock();
			}
		}
		void FillControls(Control parentControl, Action<IList<Control>> collectControls) {
			parentControl.Controls.Clear();
			List<Control> controls = new List<Control>();
			collectControls(controls);
			foreach (Control control in controls) {
				parentControl.Controls.Add(control);
				control.SendToBack();
			}
		}
		bool ChartContainsFinancialSeries() {
			bool containsFinancial = false;
			if (chart != null) {
				foreach (Series series in chart.Series)
					if (series.View is FinancialSeriesViewBase) {
						containsFinancial = true;
						break;
					}
			}
			return containsFinancial;
		}
		void CollectActiveControls(IList<Control> activeControls) {
			activeControls.Add(grpGrid);
			if (SupportsScaleMode) {
				activeControls.Add(sepGrid);
			}
			CollectActiveControlsCore(activeControls);
			if (SupportsScaleMode) {
				activeControls.Add(pnlAggregateFunction);
				activeControls.Add(pnlScaleModeSeparator);
				activeControls.Add(pnlScaleMode);
			}
		}
		void CollectActiveGridControls(IList<Control> activeControls) {
			CollectActiveGridControlsCore(activeControls);
			if (SupportsGridSpacing) {
				activeControls.Add(this.sepGridAlignment);
				activeControls.Add(this.pnlGridOffset);
				activeControls.Add(this.sepOffset);
				activeControls.Add(this.pnlGridSpacing);
				activeControls.Add(this.sepValue);
				activeControls.Add(this.pnlAuto);
			}
		}
		void UpdateGrid() {
			bool autoScale = scaleOptions.ScaleMode == ScaleMode.Automatic;
			bool autoGrid = scaleOptions.AutoGrid;
			grpGrid.Enabled = !autoScale;
			chAutoGrid.Checked = autoGrid;
			spnGridSpacing.EditValue = scaleOptions.GridSpacing;
			pnlGridSpacing.Enabled = !autoGrid;
			spnGridOffset.EditValue = scaleOptions.GridOffset;
			pnlGridOffset.Enabled = !autoGrid;
			string gridAlignment = autoGrid || autoScale ? "" : GetGridAlignmentLabelText();
			string measureUnit = autoGrid || autoScale ? "" : GetMeasureUnitLabelText();
			gridAlignment = gridAlignment.ToLower();
			measureUnit = measureUnit.ToLower();
			lblGridSpacing.Text = spacingLabelText + gridAlignment + ":";
			lblGridOffset.Text = offsetLabelText + measureUnit + ":";
		}
		void OnScaleModeChanged() {
			if (generalControl != null)
				generalControl.UpdateRangePages(Axis);
			UpdateControls();
		}
		void cmbScaleMode_SelectedIndexChanged(object sender, EventArgs e) {
			if (!Locked) {
				scaleOptions.ScaleMode = ((ScaleModeItem)cmbScaleMode.SelectedItem).ScaleMode;
				OnScaleModeChanged();
			}
		}
		void cmbAggregateFunction_SelectedIndexChanged(object sender, EventArgs e) {
			if (!Locked) {
				scaleOptions.AggregateFunction = ((AggregateFunctionItem)cmbAggregateFunction.SelectedItem).Function;
				OnScaleModeChanged();
			}
		}
		void chGridSpacingAuto_CheckedChanged(object sender, EventArgs e) {
			if (!Locked) {
				scaleOptions.AutoGrid = chAutoGrid.Checked;
				UpdateControls();
			}
		}
		void spnGridSpacing_EditValueChanged(object sender, EventArgs e) {
			if (!Locked) {
				scaleOptions.GridSpacing = Convert.ToDouble(spnGridSpacing.EditValue);
			}
		}
		void spnGridOffset_EditValueChanged(object sender, EventArgs e) {
			if (!Locked) {
				scaleOptions.GridOffset = Convert.ToDouble(spnGridOffset.EditValue);
			}
		}
	}
}
