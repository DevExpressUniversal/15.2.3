#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
using DevExpress.Data.PivotGrid;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraTab;
namespace DevExpress.ExpressApp.PivotChart.Win {
	public class WinPivotGridFieldValueDisplayTextObject : IFieldValueDisplayTextObject {
		private class WinPivotGridFieldValueDisplayTextEventArgs : FieldValueDisplayTextEventArgs {
			PivotFieldDisplayTextEventArgs args;
			public WinPivotGridFieldValueDisplayTextEventArgs(PivotFieldDisplayTextEventArgs args) {
				this.args = args;
			}
			public override PivotGridFieldBase Field { get { return args.Field; } }
			public override string DisplayText {
				get { return args.DisplayText; }
				set { args.DisplayText = value; }
			}
			public override object Value {
				get { return args.Value; }
			}
		}
		private void pivotGrid_FieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e) {
			if(FieldValueDisplayText != null) {
				FieldValueDisplayText(sender, new WinPivotGridFieldValueDisplayTextEventArgs(e));
			}
		}
		public WinPivotGridFieldValueDisplayTextObject(PivotGridControl pivotGrid) {
			pivotGrid.FieldValueDisplayText -= new PivotFieldDisplayTextEventHandler(pivotGrid_FieldValueDisplayText);
			pivotGrid.FieldValueDisplayText += new PivotFieldDisplayTextEventHandler(pivotGrid_FieldValueDisplayText);
		}
		public event EventHandler<FieldValueDisplayTextEventArgs> FieldValueDisplayText;
	}
	[ToolboxItem(false)]
	public partial class AnalysisControlWin : UserControl, IAnalysisControl, ISupportPivotGridFieldBuilder {
		private PivotGridFieldBuilder pivotGridFieldBuilder;
		private IAnalysisDataSource dataSource;
		private int updateCount = 0;
		private bool readOnly = false;
		private bool initialColumnGrandTotalEnabled = true;
		private bool initialRowGrandTotalEnabled = true;
		private bool IsUpdating { get { return updateCount > 0; } }
		private void tabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e) {
			if(e.Page == pgChart) {
				UpdateGrandTotalCheckBoxes();
			}
			OnChartVisibilityChanged();
		}
		private void pivotGrid_GridLayout(object sender, EventArgs e) {
			OnPivotGridSettingsChanged();
		}
		private void pivotGrid_FieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e) {
			if(e.Field!=null && e.Field.DataType != null && e.Field.DataType.IsEnum) {
				EnumDescriptor descriptor = new EnumDescriptor(e.Field.DataType);
				e.DisplayText = descriptor.GetCaption(e.Value);
			}
		}
		private void UpdateGrandTotalCheckBoxesEnabled() {
			showColumnGrandTotalCheckEdit.Enabled = initialColumnGrandTotalEnabled && !ReadOnly;
			showRowGrandTotalCheckEdit.Enabled = initialRowGrandTotalEnabled && !ReadOnly;
		}
		private bool isGrandTotalsUpdating = false;
		private void UpdateGrandTotalCheckBoxes() {
			if(isGrandTotalsUpdating) {
				return;
			}
			try {
				isGrandTotalsUpdating = true;
				showColumnGrandTotalCheckEdit.Checked = InitialGrandTotalHelper.Instance.GetColumnGrandTotalChecked(this, true);
				showRowGrandTotalCheckEdit.Checked = InitialGrandTotalHelper.Instance.GetRowGrandTotalChecked(this, true);
				initialColumnGrandTotalEnabled = InitialGrandTotalHelper.Instance.GetColumnGrandTotalEnabled(this);
				initialRowGrandTotalEnabled = InitialGrandTotalHelper.Instance.GetRowGrandTotalEnabled(this);
				UpdateGrandTotalCheckBoxesEnabled();
			}
			finally {
				isGrandTotalsUpdating = false;
			}
		}
		private void OnShowChartWizard(System.EventArgs e) {
			if(ShowChartWizard != null) {
				ShowChartWizard(this, e);
				OnChartSettingsChanged();
			}
		}
		private void chartControlContextMenuItem_Click(object sender, System.EventArgs e) {
			OnShowChartWizard(e);
		}
		private void showColumnGrandTotalCheckEdit_CheckedChanged(object sender, EventArgs e) {
			this.OptionsChartDataSource.ProvideColumnGrandTotals = this.showColumnGrandTotalCheckEdit.Checked;
			OnPivotGridSettingsChanged();
		}
		private void ShowRowGrandTotal_CheckedChanged(object sender, EventArgs e) {
			this.OptionsChartDataSource.ProvideRowGrandTotals = this.showRowGrandTotalCheckEdit.Checked;
			OnPivotGridSettingsChanged();
		}
		private void OptionsChartDataSource_Changed(object sender, EventArgs e) {
			if(showColumnGrandTotalCheckEdit.Checked != OptionsChartDataSource.ProvideColumnGrandTotals ||
			   showRowGrandTotalCheckEdit.Checked != OptionsChartDataSource.ProvideRowGrandTotals) {
				showColumnGrandTotalCheckEdit.Checked = OptionsChartDataSource.ProvideColumnGrandTotals;
				showRowGrandTotalCheckEdit.Checked = OptionsChartDataSource.ProvideRowGrandTotals;
				OnChartSettingsChanged();
			}
		}
		private void pivotGrid_FieldAreaChanged(object sender, PivotFieldEventArgs e) {
			UpdateGrandTotalCheckBoxes();
		}
		private void AssignChartDataSource() {
			if(ChartControl.Visible) {
				ChartControl.DataSource = PivotGrid;
			}
			else {
				ChartControl.DataSource = null;
			}
		}
		protected internal void SetObjectSpace(IObjectSpace objectSpace) {
			pivotGridFieldBuilder.SetObjectSpace(objectSpace);
		}
		protected virtual void OnPivotGridSettingsChanged() {
			if(!IsUpdating) {
				if(PivotGridSettingsChanged != null) {
					PivotGridSettingsChanged(this, EventArgs.Empty);
				}
			}
		}
		protected virtual void OnChartSettingsChanged() {
			AssignChartDataSource();
			if(ChartSettingsChanged != null)
				ChartSettingsChanged(this, EventArgs.Empty);
		}
		protected virtual void OnChartVisibilityChanged() {
			AssignChartDataSource();
			if(ChartVisibilityChanged != null) {
				ChartVisibilityChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void ApplyReadOnly() {
			PivotGrid.OptionsCustomization.BeginUpdate();
			PivotGrid.OptionsCustomization.AllowDrag = !ReadOnly;
			PivotGrid.OptionsCustomization.AllowExpand = !ReadOnly;
			PivotGrid.OptionsCustomization.AllowFilter = !ReadOnly;
			PivotGrid.OptionsCustomization.AllowHideFields = (ReadOnly) ? AllowHideFieldsType.Never : AllowHideFieldsType.Always;
			PivotGrid.OptionsCustomization.AllowSort = !ReadOnly;
			PivotGrid.OptionsCustomization.AllowSortBySummary = !ReadOnly;
			PivotGrid.OptionsCustomization.EndUpdate();
			PivotGrid.OptionsMenu.EnableHeaderMenu = !ReadOnly;
			PivotGrid.OptionsMenu.EnableHeaderAreaMenu = !ReadOnly;
			UpdateGrandTotalCheckBoxesEnabled();
		}
		public virtual PivotGridField CreatePivotGridField(string fieldCaption, string fieldName, PivotSummaryType summaryType) {
			PivotGridField pivotField = new PivotGridField();
			pivotField.Options.AllowRunTimeSummaryChange = true;
			pivotField.FieldName = fieldName;
			pivotField.Name = "Field_" + fieldName;
			pivotField.Caption = fieldCaption;
			pivotField.SummaryType = summaryType;
			return pivotField;
		}
		public event EventHandler ShowChartWizard;
		public AnalysisControlWin() {
			InitializeComponent();
			pgPivot.Text = AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.PivotTabCaption);
			pgChart.Text = AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.ChartTabCaption);
			chartControlShowWizardMenuItem.Text = AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.ChartWizardText);
			pivotGridFieldBuilder = new PivotGridFieldBuilder(this);
			pivotGridFieldBuilder.SettingsApplied += new EventHandler<EventArgs>(pivotGridFieldBuilder_SettingsApllied);
			pivotGrid.OptionsChartDataSource.SelectionOnly = false;
			pivotGrid.OptionsChartDataSource.DataProvideMode = PivotChartDataProvideMode.UseCustomSettings;
			pivotGrid.OptionsChartDataSource.OptionsChanged += new EventHandler(OptionsChartDataSource_Changed);
			pivotGrid.FieldValueDisplayText += new PivotFieldDisplayTextEventHandler(pivotGrid_FieldValueDisplayText);
			pivotGrid.GridLayout += new EventHandler(pivotGrid_GridLayout);			
			pivotGrid.OptionsBehavior.HorizontalScrolling = PivotGridScrolling.Control;
			pivotGrid.Dock = DockStyle.Fill;
			pivotGrid.OptionsCustomization.AllowPrefilter = false;
			AssignChartDataSource();
			chartControl.SeriesDataMember = "Series";
			chartControl.SeriesTemplate.ArgumentDataMember = "Arguments";
			chartControl.SeriesTemplate.ValueDataMembers.AddRange(new string[] { "Values" });
			this.showRowGrandTotalCheckEdit.Text = AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.ShowRowGrandTotalText);
			this.showColumnGrandTotalCheckEdit.Text = AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.ShowColumnGrandTotalText);
			pivotGrid.FieldAreaChanged += new PivotFieldEventHandler(pivotGrid_FieldAreaChanged);
		}
		void pivotGridFieldBuilder_SettingsApllied(object sender, EventArgs e) {
			UpdateGrandTotalCheckBoxes();
		}
		public IAnalysisDataSource DataSource {
			get { return dataSource; }
			set {
				dataSource = value;
				try {
					if(dataSource != null) {
						pivotGridFieldBuilder.RebuildFields();
						PivotGrid.DataSource = dataSource.PivotGridDataSource;
						AssignChartDataSource();
					}
					UpdateGrandTotalCheckBoxes();
				}
				catch {
					dataSource = null;
					PivotGrid.DataSource = null;
					AssignChartDataSource();
					pivotGridFieldBuilder.RebuildFields();
					throw;
				}
			}
		}
		public XtraTabControl TabControl {
			get { return tabControl; }
		}
		public PivotGridControl PivotGrid {
			get { return pivotGrid; }
		}
		public ChartControl ChartControl {
			get { return chartControl; }
		}
		public CheckEdit ShowColumnGrandTotalCheckEdit {
			get {
				return showColumnGrandTotalCheckEdit;
			}
		}
		public CheckEdit ShowRowGrandTotalCheckEdit {
			get { return showRowGrandTotalCheckEdit; }
		}
		public MenuItem ChartShowWizardMenuItem {
			get { return chartControlShowWizardMenuItem; }
		}
		public DevExpress.XtraCharts.ViewType ChartType {
			get { return SeriesViewFactory.GetViewType(chartControl.SeriesTemplate.View); }
		}
		public bool IsChartVisible {
			get { return tabControl.SelectedTabPage == pgChart; }
		}
		public PivotGridFieldBuilder FieldBuilder {
			get { return pivotGridFieldBuilder; }
			set { pivotGridFieldBuilder = value; }
		}		
		public bool ReadOnly {
			get { return readOnly; }
			set {
				if(value != readOnly) {
					readOnly = value;
					ApplyReadOnly();
				}
			}
		}		
		public event EventHandler<EventArgs> ChartVisibilityChanged;
		#region IAnalysisControl Members
		public PivotGridFieldCollectionBase Fields {
			get { return PivotGrid.Fields; }
		}
		public PivotGridOptionsChartDataSourceBase OptionsChartDataSource {
			get { return PivotGrid.OptionsChartDataSource; }
		}
		public Chart Chart {
			get { return ((IChartContainer)ChartControl).Chart; }
		}
		public void BeginUpdate() {
			updateCount++;
			if(updateCount == 1) {
				PivotGrid.BeginUpdate();
			}
		}
		public void EndUpdate() {
			if(updateCount == 1) {
				PivotGrid.EndUpdate();
				ApplyReadOnly();
			}
			updateCount--;
		}
		public void AddPivotGridField(string fieldCaption, string fieldName, PivotSummaryType summaryType) {
			pivotGrid.Fields.Add(CreatePivotGridField(fieldCaption, fieldName, summaryType));
		}
		public event EventHandler<EventArgs> PivotGridSettingsChanged;
		public event EventHandler<EventArgs> ChartSettingsChanged;
		#endregion
	}
}
