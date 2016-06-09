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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data.PivotGrid;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.ExpressApp.PivotChart.Web {
	public class ASPxPivotGridFieldValueDisplayTextObject : IFieldValueDisplayTextObject {
		private class ASPxPivotGridFieldValueDisplayTextEventArgs : FieldValueDisplayTextEventArgs {
			PivotFieldDisplayTextEventArgs args;
			public ASPxPivotGridFieldValueDisplayTextEventArgs(PivotFieldDisplayTextEventArgs args) {
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
				FieldValueDisplayText(sender, new ASPxPivotGridFieldValueDisplayTextEventArgs(e));
			}
		}
		public ASPxPivotGridFieldValueDisplayTextObject(ASPxPivotGrid pivotGrid) {
			pivotGrid.FieldValueDisplayText -= new PivotFieldDisplayTextEventHandler(pivotGrid_FieldValueDisplayText);
			pivotGrid.FieldValueDisplayText += new PivotFieldDisplayTextEventHandler(pivotGrid_FieldValueDisplayText);
		}
		public event EventHandler<FieldValueDisplayTextEventArgs> FieldValueDisplayText;
	}
	[ToolboxItem(false)]
	public class AnalysisControlWeb : WebControl, INamingContainer, IAnalysisControl, ISupportPivotGridFieldBuilder, ITestableEx, ISupportAdditionalParametersTestControl, IXafCallbackHandler {
		private const string ShowColumnGrandTotalCheckBoxName = "showColumnGrandTotalCheckBox";
		private const string ShowRowGrandTotalCheckBoxName = "showRowGrandTotalCheckBox";
		private const string ChartPageName = "ChartPage";
		private ASPxPageControl pageControl;
		private ASPxPivotGrid pivotGrid;
		private ASPxPivotGridExporter pivotGridExporter;
		private WebChartControl chartControl;
		private PivotGridFieldBuilder pivotGridFieldBuilder;
		private IAnalysisDataSource dataSource;
		private ASPxCheckBox showRowGrandTotalCheckBox;
		private ASPxCheckBox showColumnGrandTotalCheckBox;
		private ASPxComboBox chartTypeComboBox;
		private ASPxButton printChartButton;
		private bool isChartBinding = false;
		private bool isPivotBound = false;
		private bool readOnly = false;
		private void chartControl_DataBinding(object sender, EventArgs e) {
			if(!isPivotBound) {
				pivotGrid.DataBind();
			}
		}
		private void pivotGrid_DataBound(object sender, EventArgs e) {
			if(!isPivotBound && !isChartBinding) {
				isPivotBound = true;
				isChartBinding = true;
				try {
					UpdateChartDataSource();
				}
				finally {
					isChartBinding = false;
				}
			}
		}
		private void CreatePivotGridControl() {
			pivotGrid = new ASPxPivotGrid();
			pivotGrid.OptionsChartDataSource.DataProvideMode = PivotChartDataProvideMode.UseCustomSettings;
			RenderHelper.SetupASPxWebControl(pivotGrid);
			pivotGrid.ID = "PivotGrid";
			pivotGrid.EncodeHtml = true;
			pivotGrid.DataBound += new EventHandler(pivotGrid_DataBound);
			new PivotGridEnumValueLocalizer().Attach(new ASPxPivotGridFieldValueDisplayTextObject(pivotGrid));
			pivotGrid.GridLayout += new EventHandler(pivotGrid_GridLayout);
			pivotGrid.CssFilePath = "DX_PivotGrid_Styles.css";
			pivotGrid.Data.SetIsLoading(false);
			pivotGrid.Width = Unit.Percentage(100);
			pivotGrid.Unload += new EventHandler(pivotGrid_Unload);
		}
		void pivotGrid_Unload(object sender, EventArgs e) {
			OnControlInitialized();
		}
		private void pivotGrid_GridLayout(object sender, EventArgs e) {
			if(!pivotGrid.IsRendering) {
				OnPivotGridSettingsChanged();
			}
		}
		private void CreatePivotGridExporterControl() {
			pivotGridExporter = new ASPxPivotGridExporter();
			pivotGridExporter.ID = "PivotGridExporter";
			pivotGridExporter.ASPxPivotGridID = pivotGrid.ID;
		}
		private void CreateCheckBoxAndComboBox() {
			showColumnGrandTotalCheckBox = RenderHelper.CreateASPxCheckBox();
			showColumnGrandTotalCheckBox.Enabled = true;
			showColumnGrandTotalCheckBox.ID = "ShowColumnGrandTotalCheckBox";
			showColumnGrandTotalCheckBox.Style.Add("white-space", "nowrap");
			showColumnGrandTotalCheckBox.EnableClientSideAPI = true;
			showColumnGrandTotalCheckBox.ClientInstanceName = ShowColumnGrandTotalCheckBoxName;
			showColumnGrandTotalCheckBox.Text = AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.ShowColumnGrandTotalText);
			showColumnGrandTotalCheckBox.CheckedChanged += new EventHandler(showColumnGrandTotalCheckBox_CheckedChanged);
			showRowGrandTotalCheckBox = RenderHelper.CreateASPxCheckBox();
			showRowGrandTotalCheckBox.Enabled = true;
			showRowGrandTotalCheckBox.ID = "ShowRowGrandTotalCheckBox";
			showRowGrandTotalCheckBox.Style.Add("white-space", "nowrap");
			showRowGrandTotalCheckBox.EnableClientSideAPI = true;
			showRowGrandTotalCheckBox.ClientInstanceName = ShowRowGrandTotalCheckBoxName;
			showRowGrandTotalCheckBox.Text = AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.ShowRowGrandTotalText);
			showRowGrandTotalCheckBox.CheckedChanged += new EventHandler(showRowGrandTotalCheckBox_CheckedChanged);
			chartTypeComboBox = RenderHelper.CreateASPxComboBox();
			chartTypeComboBox.ID = "ChartTypeComboBox";
			chartTypeComboBox.AutoPostBack = false;
			chartTypeComboBox.EnableClientSideAPI = true;
			chartTypeComboBox.ClientInstanceName = "chartTypeComboBox";
			EnumDescriptor descriptor = new EnumDescriptor(DevExpress.XtraCharts.ViewType.Pie3D.GetType());
			chartTypeComboBox.Items.AddRange(descriptor.Values);
			chartTypeComboBox.SelectedIndexChanged += new EventHandler(chartTypeComboBox_SelectedIndexChanged);
		}
		private void CreatePrintChartButton() {
			printChartButton = RenderHelper.CreateASPxButton();
			printChartButton.ID = "PrintChartButton";
			printChartButton.ClientInstanceName = "printChartButton";
			printChartButton.EnableClientSideAPI = true;
			ASPxImageHelper.SetImageProperties(printChartButton.Image, "MenuBar_Print");
			printChartButton.ToolTip = AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.PrintChartText);
		}
		private void chartTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
			bool needSaveSettings = true;
			if(ChartTypeComboBox.SelectedIndex != -1) {
				DevExpress.XtraCharts.ViewType previuosType = ChartType;
				try {
					if(((IChartContainer)ChartControl).Chart.DataContainer.ActualDataSource == null) {
						ChartControl.DataBind();
					}
					ChartControl.SeriesTemplate.ChangeView((DevExpress.XtraCharts.ViewType)ChartTypeComboBox.SelectedIndex);
					needSaveSettings = previuosType != ChartType;
				}
				catch(Exception ex) {
					EnumDescriptor descriptor = new EnumDescriptor(DevExpress.XtraCharts.ViewType.Pie3D.GetType());
					string errorMessage = string.Format(AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.IncorrectChartTypeErrorMessage), descriptor.GetCaption(ChartTypeComboBox.SelectedIndex));
					ChartTypeComboBox.SelectedIndex = (int)previuosType;
					ErrorHandling.Instance.SetPageError(new Exception(errorMessage, ex));
				}
			}
			if(needSaveSettings) {
				OnChartSettingsChanged();
			}
		}
		private void showColumnGrandTotalCheckBox_CheckedChanged(object sender, EventArgs e) {
			pivotGrid.OptionsChartDataSource.ProvideColumnGrandTotals = showColumnGrandTotalCheckBox.Checked;
			OnPivotGridSettingsChanged();
		}
		private void showRowGrandTotalCheckBox_CheckedChanged(object sender, EventArgs e) {
			pivotGrid.OptionsChartDataSource.ProvideRowGrandTotals = ShowRowGrandTotalCheckBox.Checked;
			OnPivotGridSettingsChanged();
		}
		private void OnIsChartVisibleChanged() {
			if(IsChartVisibleChanged != null) {
				IsChartVisibleChanged(this, new HandledEventArgs());
			}
		}
		private void CreateChartControl() {
			chartControl = new WebChartControl();
			RenderHelper.SetupASPxWebControl(chartControl);
			chartControl.ID = "Chart";
			chartControl.SeriesDataMember = "Series";
			chartControl.SeriesTemplate.ArgumentDataMember = "Arguments";
			chartControl.SeriesTemplate.ValueDataMembers.AddRange(new string[] { "Values" });
			chartControl.CssClass = "xafChart";
			chartControl.Width = Unit.Pixel(700);
			chartControl.Height = Unit.Pixel(400);
			chartControl.EnableClientSideAPI = true;
			chartControl.ClientInstanceName = "ChartImage";
			chartControl.DataBinding += new EventHandler(chartControl_DataBinding);
		}
		private void CreatePageControl() {
			pageControl = RenderHelper.CreateASPxPageControl();
			pageControl.ID = "Pages";
			pageControl.SaveStateToCookies = true;
			pageControl.ContentStyle.CssClass = "TabControlContent";
			pageControl.EnableViewState = false;
			pageControl.Width = Unit.Percentage(100);
			TabPage pivotPage = pageControl.TabPages.Add(AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.PivotTabCaption));
			pivotPage.Name = "PivotPage";
			Panel pivotPanel = new Panel();
			pivotPanel.Controls.Add(pivotGridExporter);
			pivotPanel.Controls.Add(pivotGrid);
			pivotPage.Controls.Add(pivotPanel);
			TabPage chartPage = pageControl.TabPages.Add(AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.ChartTabCaption));
			TableCell chartTypeComboBoxCell = new TableCell();
			chartTypeComboBoxCell.Controls.Add(chartTypeComboBox);
			ASPxLabel chartTypeLabel = RenderHelper.CreateASPxLabel();
			chartTypeLabel.Text = AnalysisControlLocalizer.Active.GetLocalizedString(AnalysisControlId.ChartTypeCaption);
			TableCell chartTypeLabelCell = new TableCell();
			chartTypeLabelCell.Style.Add("padding-top", "5px");
			chartTypeLabelCell.CssClass = "dxeMemoEditArea_xaf";
			chartTypeLabelCell.Controls.Add(chartTypeLabel);
			TableCell showRowGrandTotalCheckBoxCell = new TableCell();
			showRowGrandTotalCheckBoxCell.Controls.Add(ShowRowGrandTotalCheckBox);
			TableCell showColumnGrandTotalCheckBoxCell = new TableCell();
			showColumnGrandTotalCheckBoxCell.Controls.Add(showColumnGrandTotalCheckBox);
			TableCell printButtonCell = new TableCell();
			printButtonCell.Controls.Add(printChartButton);
			TableRow row = new TableRow();
			row.Cells.AddRange(new TableCell[] { chartTypeLabelCell, chartTypeComboBoxCell, showRowGrandTotalCheckBoxCell, showColumnGrandTotalCheckBoxCell, printButtonCell });
			row.HorizontalAlign = HorizontalAlign.Center;
			Table table = RenderHelper.CreateTable();
			table.Rows.Add(row);
			table.CellPadding = 5;
			Panel additionalPanel = new Panel();
			additionalPanel.HorizontalAlign = HorizontalAlign.Left;
			additionalPanel.Controls.Add(table);
			chartPage.Name = ChartPageName;
			Panel chartPanel = new Panel();
			chartPanel.CssClass = "xafChartPanel";
			chartPanel.Controls.Add(chartControl);
			chartPanel.Controls.Add(additionalPanel);
			chartPage.Controls.Add(chartPanel);
		}
		private void UpdateGrandTotalCheckBoxes() {
			showColumnGrandTotalCheckBox.Checked = InitialGrandTotalHelper.Instance.GetColumnGrandTotalChecked(this, showColumnGrandTotalCheckBox.ClientEnabled);
			showRowGrandTotalCheckBox.Checked = InitialGrandTotalHelper.Instance.GetRowGrandTotalChecked(this, showRowGrandTotalCheckBox.ClientEnabled);
			showColumnGrandTotalCheckBox.ClientEnabled = InitialGrandTotalHelper.Instance.GetColumnGrandTotalEnabled(this);
			showRowGrandTotalCheckBox.ClientEnabled = InitialGrandTotalHelper.Instance.GetRowGrandTotalEnabled(this);
		}
		private void callbackManager_PreRender(object sender, EventArgs e) {
			chartTypeComboBox.SelectedIndex = (int)SeriesViewFactory.GetViewType(Chart.DataContainer.SeriesTemplate.View);
			UpdateGrandTotalCheckBoxes();
			DataBind();
					}
		private ICallbackManagerHolder CallbackManagerHolder {
			get {
				Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), Page.GetType(), "Page");
				return (ICallbackManagerHolder)WebWindow.CurrentRequestPage;
			}
		}
		protected internal void SetObjectSpace(IObjectSpace objectSpace) {
			pivotGridFieldBuilder.SetObjectSpace(objectSpace);
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			CallbackManagerHolder.CallbackManager.RegisterHandler(UniqueID, this);
		}
		protected override void OnLoad(EventArgs e) {
			XafCallbackManager callbackManager = CallbackManagerHolder.CallbackManager;
			callbackManager.PreRender += new EventHandler<EventArgs>(callbackManager_PreRender);			
			pageControl.ClientSideEvents.ActiveTabChanged = string.Format("function(s, e) {{{0}}}", callbackManager.GetScript(UniqueID, "e.tab.name"));
			chartTypeComboBox.ClientSideEvents.SelectedIndexChanged = string.Format("function(s,e){{{0} e.processOnServer=false;}}", callbackManager.GetScript());
			string checkedChangedFunction = string.Format("function(s, e) {{{0}}}", callbackManager.GetScript());
			showColumnGrandTotalCheckBox.ClientSideEvents.CheckedChanged = checkedChangedFunction;
			showRowGrandTotalCheckBox.ClientSideEvents.CheckedChanged = checkedChangedFunction;
			printChartButton.ClientSideEvents.Click = string.Format("function(s, e) {{ {0}.Print(); }}", chartControl.ClientInstanceName);
			base.OnLoad(e);
		}
		protected virtual void OnPivotGridSettingsChanged() {
			if(PivotGridSettingsChanged != null) {
				PivotGridSettingsChanged(this, EventArgs.Empty);
			}
			DataBind();
		}
		protected virtual void OnChartSettingsChanged() {
			if(ChartSettingsChanged != null) {
				ChartSettingsChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void ApplyReadOnly() {
			PivotGrid.OptionsCustomization.BeginUpdate();
			PivotGrid.OptionsCustomization.AllowDrag = !ReadOnly;
			PivotGrid.OptionsCustomization.AllowExpand = !ReadOnly;
			PivotGrid.OptionsCustomization.AllowFilter = !ReadOnly;
			PivotGrid.OptionsCustomization.AllowSort = !ReadOnly;
			PivotGrid.OptionsCustomization.AllowSortBySummary = !ReadOnly;
			showColumnGrandTotalCheckBox.ReadOnly = ReadOnly;
			showRowGrandTotalCheckBox.ReadOnly = ReadOnly;
			chartTypeComboBox.ReadOnly = ReadOnly;
			PivotGrid.OptionsCustomization.EndUpdate();
		}
		protected override void OnUnload(EventArgs e) {
			CallbackManagerHolder.CallbackManager.PreRender -= new EventHandler<EventArgs>(callbackManager_PreRender);
			base.OnUnload(e);
		}
		public override void Dispose() {
			pivotGrid.DataBound -= new EventHandler(pivotGrid_DataBound);
			chartControl.DataBinding -= new EventHandler(chartControl_DataBinding);
			base.Dispose();
		}
		public AnalysisControlWeb() {
			pivotGridFieldBuilder = new PivotGridFieldBuilder(this);
			CreatePivotGridControl();
			CreatePivotGridExporterControl();
			CreateChartControl();
			CreateCheckBoxAndComboBox();
			CreatePrintChartButton();
			CreatePageControl();
			Controls.Add(pageControl);
		}
		public void AddPivotGridField(string fieldCaption, string fieldName, PivotSummaryType summaryType) {
			PivotGridField pivotField = new PivotGridField();
			pivotField.FieldName = fieldName;
			pivotField.Caption = fieldCaption;
			pivotField.SummaryType = summaryType;
			pivotGrid.Fields.Add(pivotField);
		}
		public void UpdateChartDataSource() {
			chartControl.DataSourceID = pivotGrid.ID;
			chartControl.DataBind();
		}
		public void ForceReadOnly() {
			ApplyReadOnly();
		}
		public ASPxComboBox ChartTypeComboBox {
			get {
				return chartTypeComboBox;
			}
		}
		public ASPxCheckBox ShowColumnGrandTotalCheckBox {
			get {
				return showColumnGrandTotalCheckBox;
			}
		}
		public ASPxCheckBox ShowRowGrandTotalCheckBox {
			get {
				return showRowGrandTotalCheckBox;
			}
		}
		public ASPxPageControl PageControl {
			get { return pageControl; }
		}
		public ASPxPivotGrid PivotGrid {
			get { return pivotGrid; }
		}
		public ASPxPivotGridExporter PivotGridExporter {
			get { return pivotGridExporter; }
		}
		public WebChartControl ChartControl {
			get { return chartControl; }
		}
		public DevExpress.XtraCharts.ViewType ChartType {
			get { return SeriesViewFactory.GetViewType(chartControl.SeriesTemplate.View); }
		}
		public IAnalysisDataSource DataSource {
			get { return dataSource; }
			set {
				dataSource = value;
				if(dataSource != null) {
					pivotGridFieldBuilder.RebuildFields();
					PivotGrid.DataSource = dataSource.PivotGridDataSource;
				}
			}
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
		public bool IsChartVisible {
			get { return PageControl.ActiveTabPage != null && PageControl.ActiveTabPage.Name == ChartPageName; }
			set {
				PageControl.ActiveTabIndex = value ? 1 : 0; 
				OnIsChartVisibleChanged();
			}
		}
		public event EventHandler<HandledEventArgs> IsChartVisibleChanged;
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
			PivotGrid.BeginUpdate();
		}
		public void EndUpdate() {
			PivotGrid.EndUpdate();
		}
		private bool isDataBinding = false;
		public override void DataBind() {
			if(pivotGrid != null && chartControl != null && pivotGrid.Page != null && !isDataBinding) {
				try {
					isDataBinding = true;
					pivotGrid.DataBind();
				}
				finally {
					isDataBinding = false;
				}
			}
		}
		public event EventHandler<EventArgs> PivotGridSettingsChanged;
		public event EventHandler<EventArgs> ChartSettingsChanged;
		#endregion
		protected void OnControlInitialized() {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(PivotGrid));
			}
		}
		#region ITestableEx Members
		public Type RegisterControlType {
			get { return PivotGrid.GetType(); }
		}
		#endregion
		#region ITestable Members
		public string TestCaption {
			get { return "PivotGrid"; }
		}
		public string ClientId {
			get { return PivotGrid.ClientID; }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public IJScriptTestControl TestControl {
			get { return null; }
		}
		public virtual TestControlType TestControlType {
			get {
				return TestControlType.Table;
			}
		}
		#endregion
		#region ISupportAdditionalParametersTestControl Members
		public ICollection<string> GetAdditionalParameters(object control) {
			List<string> fieldCaptionParameters = new List<string>();
			foreach(PivotGridField field in PivotGrid.Fields) {
				if(field.Visible) {
					fieldCaptionParameters.Add(string.Format("{0}={1}", field.Caption, PivotGrid.ScriptHelper.GetHeaderID(PivotGrid.Data.GetFieldItem(field))));
				}
			}
			string fieldCaptionsParameter = string.Format("'{0}'", string.Join(";", fieldCaptionParameters.ToArray()));
			return new string[] { fieldCaptionsParameter };
		}
		#endregion
		#region IXafCallbackHandler Members
		void IXafCallbackHandler.ProcessAction(string parameter) {
			IsChartVisible = parameter == "ChartPage";
		}
		#endregion
	}
}
