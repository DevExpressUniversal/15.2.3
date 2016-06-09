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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
using DevExpress.Utils;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class DataBindingControl : InternalWizardControlBase {
		public static bool ValidateArgumentScaleType(PopupBaseEdit edit, SeriesBase series, ScaleType scaleType) {
			if (series == null)
				return true; 
			try {
				SeriesScaleTypeUtils.ValidatePointsByArgumentScaleType(series, scaleType);
			}
			catch (Exception ex) {
				edit.ErrorText = ex.Message;
				return false;
			}
			return true;
		}
		public static bool ValidateValueScaleType(PopupBaseEdit edit, SeriesBase series, ScaleType scaleType) {
			if (series == null)
				return true; 
			try {
				SeriesScaleTypeUtils.ValidatePointsByValueScaleType(series, scaleType);
			}
			catch (Exception ex) {
				edit.ErrorText = ex.Message;
				return false;
			}
			return true;
		}
		WizardFormLayout layout;
		SeriesBase series = null;
		Chart chart;
		PopupContainerEdit currentPopupEdit = null;
		bool isSummaryBinding = false;
		bool isDesignTime = false;
		bool isEndUserDesigner = false;
		IViewArgumentValueOptions View { get { return series.View; } }
		DataContext DataContext { 
			get {
				IChartContainer chartContainer = CommonUtils.FindChartContainer(chart);
				return chartContainer !=null && chartContainer.DataProvider != null ?  chartContainer.DataProvider.DataContext : null; } 
		}
		object DataSource { get { return series == null ? null : SeriesDataBindingUtils.GetDataSource(series); } }
		bool DataSourceExists { get { return DataSource != null; } }
		string ChartDataMember { get { return series == null ? null : SeriesDataBindingUtils.GetDataMember(series); } }
		ScaleType[] ArgumentScaleTypes {
			get { 
				return (series == null || series.View == null || View.NonNumericArgumentSupported) ?
					new ScaleType[0] : new ScaleType[] { ScaleType.Numerical };
			}
		}
		ScaleType[] ColorScaleTypes {
			get {
				return new ScaleType[] { ScaleType.Numerical, ScaleType.Qualitative };
			}
		}
		ScaleType[] ValueScaleTypes {
			get {
				return (series == null || series.View == null || View.DateTimeValuesSupported) ?
					new ScaleType[] { ScaleType.Numerical, ScaleType.DateTime } : new ScaleType[] { ScaleType.Numerical };
			}
		}
		bool LockFunctionNameEditing { get { return !isDesignTime || isEndUserDesigner; } }
		public event EventHandler ValuableBindingChanged;
		public DataBindingControl() {
			InitializeComponent();
			UpdateControls();
		}
		bool IsSmartAutoBindingSettingsDisabled(SeriesBase series) {
			return series == null || chart == null || !PivotGridDataSourceUtils.IsAutoBindingSettingsUsed(chart.DataContainer.PivotGridDataSourceOptions, series);
		}
		void RaiseValuableBindingChangedEvent() {
			if (ValuableBindingChanged != null)
				ValuableBindingChanged(this, EventArgs.Empty);
		}
		void UpdateScaleTypeControls() {
			if (series == null) {
				cbArgumentScaleType.Enabled = false;
				cbArgumentScaleType.SelectedIndex = -1;
				cbValueScaleType.Enabled = false;
				cbValueScaleType.SelectedIndex = -1;
				pnlValueScaleType.Enabled = true;
			}
			else {
				cbArgumentScaleType.Enabled = true;
				cbArgumentScaleType.SelectedIndex = WizardScaleTypeHelper.GetArgumentScaleTypeIndex(series.ArgumentScaleType);
				cbValueScaleType.Enabled = true;
				cbValueScaleType.SelectedIndex = WizardScaleTypeHelper.GetValueScaleTypeIndex(series.ValueScaleType);
				pnlValueScaleType.Enabled = IsSmartAutoBindingSettingsDisabled(series);
			}
		}
		void UpdateArgumentBindingControls() {
			cbArgument.Enabled = DataSourceExists;
			cbArgument.EditValue = (series == null || String.IsNullOrEmpty(series.ArgumentDataMember)) ? String.Empty : series.ArgumentDataMember;
		}
		void UpdateColorBindingControls() {
			cbColor.Enabled = DataSourceExists;
			cbColor.EditValue = (series == null || String.IsNullOrEmpty(series.ColorDataMember)) ? String.Empty : series.ColorDataMember;
		}
		void UpdateSimpleBindingControls() {
			bool dataSourceExists = DataSourceExists;
			pnlValue1.Visible = true;
			cbValue1.Enabled = dataSourceExists;
			cbValue2.Enabled = dataSourceExists;
			cbValue3.Enabled = dataSourceExists;
			cbValue4.Enabled = dataSourceExists;
			if (series == null) {
				pnlValue2.Visible = false;
				pnlValue3.Visible = false;
				pnlValue4.Visible = false;
				lblDockOffset2.Visible = false;
				lblDockOffset3.Visible = false;
				lblDockOffset4.Visible = false;
				cbValue1.EditValue = String.Empty;
			}
			else {
				int valuesCount = View.PointDimension;
				lblValue1.Text = series.View.GetValueCaption(0) + ":";
				cbValue1.EditValue = series.ValueDataMembers[0];
				if (valuesCount > 1) {
					pnlValue2.Visible = true;
					lblDockOffset2.Visible = true;
					lblValue2.Text = series.View.GetValueCaption(1) + ":";
					cbValue2.EditValue = series.ValueDataMembers[1];
				}
				else {
					pnlValue2.Visible = false;
					lblDockOffset2.Visible = false;
				}
				if (valuesCount > 2) {
					pnlValue3.Visible = true;
					lblDockOffset3.Visible = true;
					lblValue3.Text = series.View.GetValueCaption(2) + ":";
					cbValue3.EditValue = series.ValueDataMembers[2];
				}
				else {
					pnlValue3.Visible = false;
					lblDockOffset3.Visible = false;
				}
				if (valuesCount > 3) {
					pnlValue4.Visible = true;
					lblDockOffset4.Visible = true;
					lblValue4.Text = series.View.GetValueCaption(3) + ":";
					cbValue4.EditValue = series.ValueDataMembers[3];
				}
				else {
					pnlValue4.Visible = false;
					lblDockOffset4.Visible = false;
				}
				pnlSummaryFunction.Visible = false;
			}
			bool isAutoBindingSettingsNotUsed = IsSmartAutoBindingSettingsDisabled(series);
			pnlValue1.Enabled = isAutoBindingSettingsNotUsed;
			pnlValue2.Enabled = isAutoBindingSettingsNotUsed;
			pnlValue3.Enabled = isAutoBindingSettingsNotUsed;
			pnlValue4.Enabled = isAutoBindingSettingsNotUsed;
		}
		void UpdateSummaryBindingControls() {
			pnlValue1.Visible = false;
			pnlValue2.Visible = false;
			pnlValue3.Visible = false;
			pnlValue4.Visible = false;
			lblDockOffset2.Visible = false;
			lblDockOffset3.Visible = false;
			lblDockOffset4.Visible = false;
			pnlSummaryFunction.Visible = true;
			if (series == null) {
				pnlSummaryFunction.Enabled = false;
				beSummaryFunction.EditValue = String.Empty;
			}
			else {
				pnlSummaryFunction.Enabled = true;
				if (!String.IsNullOrEmpty(series.SummaryFunction))
					beSummaryFunction.EditValue = series.SummaryFunction;
			}
		}
		void UpdateBindingControls() {
			grValueSettings.SuspendLayout();
			try {
				UpdateArgumentBindingControls();
				UpdateColorBindingControls();
				if (isSummaryBinding)
					UpdateSummaryBindingControls();
				else
					UpdateSimpleBindingControls();
			}
			finally {
				grValueSettings.ResumeLayout();
			}
		}
		void UpdateControls() {
			bool enabled = series != null;
			grArgumentSettings.Enabled = enabled;
			pnlArgumentScaleType.Enabled = IsSmartAutoBindingSettingsDisabled(series);
			pnlArgument.Enabled = IsSmartAutoBindingSettingsDisabled(series);
			grValueSettings.Enabled = enabled;
			UpdateScaleTypeControls();
			if (enabled)
				isSummaryBinding = !String.IsNullOrEmpty(series.SummaryFunction);
			cbBindingMode.SelectedIndex = isSummaryBinding ? 1 : 0;
			UpdateBindingControls();
		}
		bool ValidateValueDataMember(int index, PopupBaseEdit edit) {
			string dataMember = (string)edit.EditValue;
			if (String.IsNullOrEmpty(dataMember))
				return true;
			int count = series.ValueDataMembers.Count;
			if (index >= count)
				return true;
			object dataSource = SeriesDataBindingUtils.GetDataSource(series);
			ScaleType scaleType = GetScaleType(dataSource, dataMember);
			if ((scaleType == ScaleType.Auto) || (scaleType == ScaleType.Qualitative))
				return false;
			for (int i = 0; i < count; i++) {
				string currentDataMember = series.ValueDataMembers[i];
				if (i != index && !String.IsNullOrEmpty(currentDataMember)) {
					ScaleType currentScaleType = GetScaleType(dataSource, currentDataMember);
					if (scaleType != currentScaleType) {
						edit.ErrorText = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncompatibleValueDataMember), 
							dataMember, ScaleTypeUtils.GetName(currentScaleType));
						return false;
					}
				}
			}
			return true;
		}
		void SetValueDataMember(int index, PopupContainerEdit edit) {
			if (series != null) {
				string dataMember = (string)edit.EditValue;
				if (String.IsNullOrEmpty(dataMember))
					series.ValueDataMembers[index] = String.Empty;
				else {
					ScaleType scaleType = GetScaleType(DataSource, dataMember);
					series.ValueDataMembers[index] = String.Empty;
					series.ValueScaleType = scaleType;
					series.ValueDataMembers[index] = (string)edit.EditValue;
					UpdateScaleTypeControls();
				}
			}
		}
		void CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			e.DisplayText = BindingHelper.GetDataMemberName(DataContext, DataSource, ChartDataMember, (string)e.Value);
		}
		void QueryResultValue(object sender, QueryResultValueEventArgs e) {
			e.Value = BindingHelper.ExtractDataMember(GetPickerResult(true), ChartDataMember);
		}
		void CloseUp(object sender, CloseUpEventArgs e) {
			StopDataMemberPicker();
		}
		void cbArgumentScaleType_Validating(object sender, CancelEventArgs e) {
			if (series != null) {
				SetInvalidState();
				e.Cancel = !ValidateArgumentScaleType((PopupBaseEdit)sender, series,
					WizardScaleTypeHelper.GetArgumentScaleType(cbArgumentScaleType.SelectedIndex));
			}
		}
		void cbArgumentScaleType_Validated(object sender, EventArgs e) {
			if (series != null) {
				series.ArgumentScaleType = WizardScaleTypeHelper.GetArgumentScaleType(cbArgumentScaleType.SelectedIndex);
				RaiseValuableBindingChangedEvent();
				SetValidState();
			}
		}
		void cbArgument_QueryPopUp(object sender, CancelEventArgs e) {
			StartDataMemberPicker(cbArgument, ArgumentScaleTypes, false);
		}
		void cbArgument_Validating(object sender, CancelEventArgs e) {
			string dataMember = (string)cbArgument.EditValue;
			if (series != null && !String.IsNullOrEmpty(dataMember)) {
				SetInvalidState();
				string savedArgument = series.ArgumentDataMember;
				ScaleType savedScaleType = series.ArgumentScaleType;
				object dataSource = SeriesDataBindingUtils.GetDataSource(series);
				try {
					series.ArgumentDataMember = String.Empty;
					series.ArgumentScaleType = GetScaleType(dataSource, dataMember);
					series.ArgumentDataMember = dataMember;
				}
				catch (Exception ex) {
					e.Cancel = true;
					cbArgument.ErrorText = ex.Message;
				}
				finally {
					series.ArgumentDataMember = String.Empty;
					series.ArgumentScaleType = savedScaleType;
					series.ArgumentDataMember = savedArgument;
				}
			}
		}
		void cbArgument_Validated(object sender, EventArgs e) {
			if (series != null) {
				series.ArgumentDataMember = String.Empty;
				string dataMember = (string)cbArgument.EditValue;
				if (!String.IsNullOrEmpty(dataMember)) {
					series.ArgumentScaleType = GetScaleType(SeriesDataBindingUtils.GetDataSource(series), dataMember);
					UpdateScaleTypeControls();
				}
				series.ArgumentDataMember = dataMember;
				RaiseValuableBindingChangedEvent();
				SetValidState();
			}
		}
		void cbColor_QueryPopUp(object sender, CancelEventArgs e) {
			StartDataMemberPicker(cbColor, ColorScaleTypes, false);
		}
		void cbColor_Validating(object sender, CancelEventArgs e) {
			string colorMember = (string)cbColor.EditValue;
			if (series != null && !String.IsNullOrEmpty(colorMember)) {
				SetInvalidState();
				string savedColor = series.ColorDataMember;				
				object dataSource = SeriesDataBindingUtils.GetDataSource(series);
				try {
					series.ColorDataMember = String.Empty;
					series.ColorDataMember = colorMember;
				}
				catch (Exception ex) {
					e.Cancel = true;
					cbArgument.ErrorText = ex.Message;
				}
				finally {
					series.ColorDataMember = String.Empty;
					series.ColorDataMember = savedColor;
				}
			}
		}
		void cbColor_Validated(object sender, EventArgs e) {
			if (series != null) {
				series.ColorDataMember = String.Empty;
				series.ColorDataMember = (string)cbColor.EditValue;
				RaiseValuableBindingChangedEvent();
				SetValidState();
			}
		}
		void cbValueScaleType_Validating(object sender, CancelEventArgs e) {
			if (series != null) {
				SetInvalidState();
				e.Cancel = !ValidateValueScaleType((PopupBaseEdit)sender, series,
					WizardScaleTypeHelper.GetValueScaleType(cbValueScaleType.SelectedIndex)); 
			}
		}
		void cbValueScaleType_Validated(object sender, EventArgs e) {
			if (series != null) {
				series.ValueScaleType = WizardScaleTypeHelper.GetValueScaleType(cbValueScaleType.SelectedIndex);
				RaiseValuableBindingChangedEvent();
				SetValidState();
			}
		}
		void cbBindingMode_SelectedIndexChanged(object sender, EventArgs e) {
			if (series != null) {
				isSummaryBinding = cbBindingMode.SelectedIndex == 1;
				if (!isSummaryBinding) 
					series.SummaryFunction = String.Empty;
				UpdateBindingControls();
			}
		}
		void cbValue1_QueryPopUp(object sender, CancelEventArgs e) {
			StartDataMemberPicker(cbValue1, ValueScaleTypes, false);
		}
		void cbValue1_Validating(object sender, CancelEventArgs e) {
			if (series != null) {
				SetInvalidState();
				e.Cancel = !ValidateValueDataMember(0, cbValue1);
			}
		}
		void cbValue1_Validated(object sender, EventArgs e) {
   			SetValueDataMember(0, cbValue1);
			SetValidState();
		}
		void cbValue2_QueryPopUp(object sender, CancelEventArgs e) {
			StartDataMemberPicker(cbValue2, ValueScaleTypes, false);
		}
		void cbValue2_Validating(object sender, CancelEventArgs e) {
			if (series != null) {
				SetInvalidState();
				e.Cancel = !ValidateValueDataMember(1, cbValue2);
			}
		}
		void cbValue2_Validated(object sender, EventArgs e) {
   			SetValueDataMember(1, cbValue2);
			SetValidState();
		}
		void cbValue3_QueryPopUp(object sender, CancelEventArgs e) {
			StartDataMemberPicker(cbValue3, ValueScaleTypes, false);
		}
		void cbValue3_Validating(object sender, CancelEventArgs e) {
			if (series != null) {
				SetInvalidState();
				e.Cancel = !ValidateValueDataMember(2, cbValue3);
			}
		}
		void cbValue3_Validated(object sender, EventArgs e) {
   			SetValueDataMember(2, cbValue3);
			SetValidState();
		}
		void cbValue4_QueryPopUp(object sender, CancelEventArgs e) {
			StartDataMemberPicker(cbValue4, ValueScaleTypes, false);
		}
		void cbValue4_Validating(object sender, CancelEventArgs e) {
			if (series != null) {
				SetInvalidState();
				e.Cancel = !ValidateValueDataMember(3, cbValue4);
			}
		}
		void cbValue4_Validated(object sender, EventArgs e) {
   			SetValueDataMember(3, cbValue4);
			SetValidState();
		}
		void beSummaryFunction_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			string text = (string)e.Value;
			try {
				e.DisplayText = new SummaryFunctionParser(text).GetDisplayText(CommonUtils.GetSummaryFunctions(chart),
					DataContext, DataSource, ChartDataMember);
			}
			catch {
				e.DisplayText = text;
			}
		}
		void beSummaryFunction_Validating(object sender, CancelEventArgs e) {
			if (series != null) {
				SetInvalidState();
				string oldSummaryFunction = series.SummaryFunction;
				try {
					series.SummaryFunction = (string)beSummaryFunction.EditValue;
					if (!isDesignTime && !String.IsNullOrEmpty(series.SummaryFunction)) {
						SummaryFunctionParser parser = new SummaryFunctionParser(series.SummaryFunction);
						SummaryFunctionsStorage summaryFunctions = CommonUtils.GetSummaryFunctions(chart);
						if (summaryFunctions[parser.FunctionName] == null) {
							String message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgSummaryFunctionIsNotRegistered), 
								parser.FunctionName);
							throw new ArgumentException(message);
						}
					}
				}
				catch (Exception exc) {
					beSummaryFunction.ErrorText = exc.Message;
					e.Cancel = true;
				}
				finally {
					series.SummaryFunction = oldSummaryFunction;
				}
			}
		}
		void beSummaryFunction_Validated(object sender, EventArgs e) {
			if (series != null) {
				string summaryFunction = (string)beSummaryFunction.EditValue;
				series.SummaryFunction = summaryFunction == null ? String.Empty : summaryFunction;
				SetValidState();
			}
		}
		void beSummaryFunction_ButtonClick(object sender, ButtonPressedEventArgs e) {
			if (!beSummaryFunction.DoValidate())
				return;
			using (SummaryFunctionEditorForm form = new SummaryFunctionEditorForm()) {
				form.Initialize(series, chart, LockFunctionNameEditing, serviceProvider);
				form.StartPosition = FormStartPosition.Manual;
				form.Location = ControlUtils.CalcLocation(Cursor.Position, Cursor.Position, form.Size);
				form.ShowDialog();
				UpdateBindingControls();
			}
		}
		void dataMemberPicker_TreeViewDoubleClick(object sender, EventArgs e) {
			if (currentPopupEdit != null)
				currentPopupEdit.ClosePopup();
		}
		ScaleType GetScaleType(object dataSource, string dataMember) {
			string actualDataMamber = BindingProcedure.ConvertToActualDataMember(ChartDataMember, dataMember);
			return WizardScaleTypeHelper.GetScaleType(DataContext, dataSource, actualDataMamber);
		}
		public void Initialize(WizardFormLayout layout, Chart chart, IServiceProvider serviceProvider) {
			this.layout = layout;
			this.chart = chart;
			this.serviceProvider = serviceProvider;
			this.dataMemberPicker.SetServiceProvider(serviceProvider);
			isDesignTime = layout.DesignTime;
			isEndUserDesigner = layout.IsEndUserDesigner;
			beSummaryFunction.Properties.TextEditStyle = LockFunctionNameEditing ? TextEditStyles.DisableTextEditor : TextEditStyles.Standard;
			if (layout.DataContainerSize != Size.Empty)
				dataMemberContainer.Size = layout.DataContainerSize;
		}
		public void Shutdown() {
			if (layout != null)
				layout.DataContainerSize = dataMemberContainer.Size;
		}
		public void SetSeries(SeriesBase series) {
			this.series = series;
			UpdateControls();
		}
		public void StartDataMemberPicker(PopupContainerEdit edit, ScaleType[] filterScaleTypes, bool filterOnlyLists) {
			currentPopupEdit = edit;
			edit.Properties.PopupControl = dataMemberContainer;
			dataMemberPicker.SetFilterCriteria(filterScaleTypes, filterOnlyLists);
			dataMemberPicker.FillDataSource(DataSource, ChartDataMember);
			dataMemberPicker.Start();
			string dataMember = (string)edit.EditValue;
			if (String.IsNullOrEmpty(dataMember))
				dataMemberPicker.SelectNoneDataMember();
			else
				dataMemberPicker.SelectDataMember(DataSource,
					filterOnlyLists ? dataMember : BindingProcedure.ConvertToActualDataMember(ChartDataMember, dataMember));
		}
		public void StopDataMemberPicker() {
			dataMemberPicker.Stop();
			currentPopupEdit = null;
		}
		public string GetPickerResult(bool onlyDataMembers) {
			return (!onlyDataMembers || dataMemberPicker.IsDataMemberNode) ? dataMemberPicker.DataMember : String.Empty;
		}
	}
}
