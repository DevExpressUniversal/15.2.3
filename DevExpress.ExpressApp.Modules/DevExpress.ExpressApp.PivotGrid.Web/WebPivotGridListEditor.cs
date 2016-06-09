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
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Utils.Serializing;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPrinting;
namespace DevExpress.ExpressApp.PivotGrid.Web {
	public class PivotCellStyleAppearanceAdapter : IAppearanceFormat {
		protected PivotCellStyle cellStyle;
		public PivotCellStyleAppearanceAdapter(PivotCellStyle cellStyle) {
			this.cellStyle = cellStyle;
		}
		#region IAppearanceFormat Members
		public FontStyle FontStyle {
			get { return FontStyle.Regular; }
			set {
				cellStyle.Font.Bold = ((value & FontStyle.Bold) == FontStyle.Bold);
				cellStyle.Font.Italic = ((value & FontStyle.Italic) == FontStyle.Italic);
				cellStyle.Font.Underline = ((value & FontStyle.Underline) == FontStyle.Underline);
				cellStyle.Font.Strikeout = ((value & FontStyle.Strikeout) == FontStyle.Strikeout);
			}
		}
		public Color FontColor {
			get { return cellStyle.ForeColor; }
			set { cellStyle.ForeColor = value; }
		}
		public Color BackColor {
			get { return cellStyle.BackColor; }
			set { cellStyle.BackColor = value; }
		}
		public void ResetFontStyle() { }
		public void ResetFontColor() { }
		public void ResetBackColor() { }
		#endregion
	}
	public class ASPxPivotGridListEditor : PivotGridListEditorBase, IExportable, ISupportPager, ISupportFilter, ISupportModelSaving {
		private ResizableControlContainer controlContainer;
		PivotGridPopupMenuManager popupMenuManager;
		private ASPxPivotGridExporter exporter;
		private bool hasStateOnClientSide;
		private string pivotGridControlCallbackState;
		protected void PivotGridControl_DataBound(object sender, EventArgs e) {
			ApplyPagerModelSettingsToControl();
			ApplyFilterModelSettingsToControl();
		}
		private void ApplyPagerModelSettingsToControl() {
			if(Model != null && PivotGridControl != null) {
				CancelEventArgs args = new CancelEventArgs();
				if(PagerSettingsApplying != null) {
					PagerSettingsApplying(this, args);
				}
				if(!args.Cancel) {
					IModelListViewWeb listViewWebModel = (IModelListViewWeb)Model;
					PivotGridControl.OptionsPager.PageIndex = listViewWebModel.PageIndex;
					PivotGridControl.OptionsPager.RowsPerPage = listViewWebModel.PageSize > 0 ? listViewWebModel.PageSize : PagerModelSynchronizer.DefaultPageSize;
					if(PagerSettingsApplied != null) {
						PagerSettingsApplied(this, EventArgs.Empty);
					}
				}
			}
		}
		private void SavePagerSettingsToModel() {
			if(Model != null && PivotGridControl != null) {
				CancelEventArgs args = new CancelEventArgs();
				if(PagerSettingsSaving != null) {
					PagerSettingsSaving(this, args);
				}
				if(!args.Cancel) {
					IModelListViewWeb listViewWebModel = (IModelListViewWeb)Model;
					listViewWebModel.PageIndex = PivotGridControl.OptionsPager.PageIndex;
					listViewWebModel.PageSize = PivotGridControl.OptionsPager.RowsPerPage;
				}
			}
		}
		private void ApplyFilterModelSettingsToControl() {
			if(Model != null && PivotGridControl != null) {
				CancelEventArgs args = new CancelEventArgs();
				if(FilterSettingsApplying != null) {
					FilterSettingsApplying(this, args);
				}
				if(!args.Cancel) {
					PivotGridControl.Prefilter.CriteriaString = Model.Filter;
					PivotGridControl.Prefilter.Enabled = Model.FilterEnabled;
				}
			}
		}
		private void SaveFilterSettingsToModel() {
			if(Model != null && PivotGridControl != null) {
				CancelEventArgs args = new CancelEventArgs();
				if(FilterSettingsSaving != null) {
					FilterSettingsSaving(this, args);
				}
				if(!args.Cancel) {
					Model.Filter = PivotGridControl.Prefilter.CriteriaString;
					Model.FilterEnabled = PivotGridControl.Prefilter.Enabled;
				}
			}
		}
		private void controlContainer_Callback(object sender, EventArgs e) {
			RefreshChart();
		}
		private void pivotGridControl_CustomCellDisplayText(object sender, DevExpress.Web.ASPxPivotGrid.PivotCellDisplayTextEventArgs e) {
			string displayText = GetDisplayText(e.DataField, e.Value);
			if(!string.IsNullOrEmpty(displayText)) {
				e.DisplayText = displayText;
			}
		}
		private void pivotGridControl_FieldValueDisplayText(object sender, DevExpress.Web.ASPxPivotGrid.PivotFieldDisplayTextEventArgs e) {
			string displayText = GetDisplayText(e.Field, e.Value);
			if(!string.IsNullOrEmpty(displayText)) {
				e.DisplayText = displayText;
			}
		}
		private bool isBinding = false;
		private void pivotGridControl_Load(object sender, EventArgs e) {
			if(pivotGridControl != null) {
				if(!isBinding) {
					isBinding = true;
					try {
						PivotGridControl.DataSource = DataSource;
						PivotGridControl.DataBind();
					}
					finally {
						isBinding = false;
					}
				}
			}
		}
		private void PivotGridControl_Init(object sender, EventArgs e) {
			if(WebWindow.CurrentRequestWindow != null) {
				WebWindow.CurrentRequestWindow.PagePreRender += new EventHandler(CurrentRequestWindow_PagePreRender);
			}
		}
		private void CurrentRequestWindow_PagePreRender(object sender, EventArgs e) {
			((WebWindow)sender).PagePreRender -= new EventHandler(CurrentRequestWindow_PagePreRender);
			if(ChartControl != null && ChartControl.Visible) {
				PivotGridControl.ClientSideEvents.BeginCallback = string.Format(@"function(s, e){{ window.{0}_IsNeedUpdateChart = true; }}", ChartControl.ClientID);
				PivotGridControl.ClientSideEvents.EndCallback = string.Format(@"function(s, e) {{ 
                    if (window.{0}_IsNeedUpdateChart){{
                        window.setTimeout(""{1}"", 1);
                        window.{0}_IsNeedUpdateChart = false;
                    }} 
                }}", ChartControl.ClientID, controlContainer.GetScript());
			}
		}
		private void RefreshChart() {
			if(ChartControl != null && ChartControl.Visible) {
				ChartControl.DataSource = pivotGridControl;
				ChartControl.SeriesDataMember = "Series";
				ChartControl.SeriesTemplate.ArgumentDataMember = "Arguments";
				ChartControl.SeriesTemplate.ValueDataMembers.AddRange(new string[] { "Values" });
				ChartControl.DataBind();
			}
		}
		protected void OnPrintableChanged() {
			if(PrintableChanged != null) {
				PrintableChanged(this, new PrintableChangedEventArgs(Printable));
			}
		}
		protected override void LoadPivotSettings(string settings) {
			bool enableSettingsLoading = !this.hasStateOnClientSide || !(WebWindow.CurrentRequestPage != null && WebWindow.CurrentRequestPage.IsPostBack);
			if((pivotGridControl != null) && !string.IsNullOrEmpty(settings) && enableSettingsLoading) {
				InitializeOptions(PivotGridControl.OptionsLayout);
				if(settings.Contains(@"application=""PivotGrid""")) {
					MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(settings));
					XmlXtraSerializer serializer = new XmlXtraSerializer();
					serializer.DeserializeObject(pivotGridControl, ms, "PivotGrid");
				}
				else {
					PivotGridControl.LoadLayoutFromString(settings);
				}
				PivotGridControl.OptionsCustomization.AllowPrefilter = false;
			}
		}
		protected override string GetPivotGridSettings() {
			return PivotGridControl.SaveLayoutToString();
		}
		protected override PivotGridFieldBase CreatePivotGridField(string propertyName, PivotArea pivotArea) {
			return new DevExpress.Web.ASPxPivotGrid.PivotGridField(propertyName, pivotArea);
		}
		protected override bool TryUpdateControlDataSource() {
			bool result = base.TryUpdateControlDataSource();
			RefreshChart();
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual ASPxPivotGrid CreateASPxPivotGrid() {
			return new ASPxPivotGrid();
		}
		protected override object CreateControlsCore() {
			popupMenuManager = new PivotGridPopupMenuManager();
			Panel panel = new Panel();
			panel.ID = "PivotPanel";
			this.pivotGridControl = CreateASPxPivotGrid();
			PivotGridControl.ID = "Pivot";
			PivotGridControl.EnableCallBacks = true;
			PivotGridControl.EncodeHtml = true;
			PivotGridControl.Width = new Unit(100, UnitType.Percentage);
			PivotGridControl.Load += new EventHandler(pivotGridControl_Load);
			popupMenuManager.Attach(PivotGridControl, PivotSettings);
			PivotGridControl.FieldValueDisplayText += new DevExpress.Web.ASPxPivotGrid.PivotFieldDisplayTextEventHandler(pivotGridControl_FieldValueDisplayText);
			PivotGridControl.CustomCellDisplayText += new DevExpress.Web.ASPxPivotGrid.PivotCellDisplayTextEventHandler(pivotGridControl_CustomCellDisplayText);
			PivotGridControl.CustomSaveCallbackState += new PivotGridCallbackStateEventHandler(PivotGridControl_CustomSaveCallbackState);
			PivotGridControl.CustomLoadCallbackState += PivotGridControl_CustomLoadCallbackState;
			PivotGridControl.Init += new EventHandler(PivotGridControl_Init);
			PivotGridControl.GridLayout += PivotGridControl_GridLayout;
			PivotGridControl.DataBound += PivotGridControl_DataBound;
			InitializeOptions(PivotGridControl.OptionsLayout);
			InitializePivotGridData(PivotGridControl.Data);
			panel.Controls.Add(PivotGridControl);
			this.chartControl = new WebChartControl();
			ChartControl.ID = "ChartForPivot";
			controlContainer = new ResizableControlContainer(ChartControl);
			controlContainer.Callback += controlContainer_Callback;
			UpdateChartVisibility(PivotSettings.ShowChart);
			panel.Controls.Add(controlContainer.Container);
			exporter = new ASPxPivotGridExporter();
			exporter.ASPxPivotGridID = PivotGridControl.ID;
			panel.Controls.Add(exporter);
			OnPrintableChanged();
			return panel;
		}
		private void PivotGridControl_GridLayout(object sender, EventArgs e) {
			OnISupportModelSaving();
		}
		protected virtual void OnISupportModelSaving() {
			if(supportModelSavingEvent != null) {
				supportModelSavingEvent(this, EventArgs.Empty);
			}
		}
		private void PivotGridControl_CustomLoadCallbackState(object sender, PivotGridCallbackStateEventArgs e) {
			e.CallbackState = pivotGridControlCallbackState;
			e.Handled = true;
		}
		private void PivotGridControl_CustomSaveCallbackState(object sender, PivotGridCallbackStateEventArgs e) {
			this.hasStateOnClientSide = true;
			pivotGridControlCallbackState = e.CallbackState;
			e.Handled = true;
		}
		public ASPxPivotGridListEditor(IModelListView model)
			: base(model) {
			this.hasStateOnClientSide = false;
		}
		public override void ApplyModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelApplying(args);
				if(!args.Cancel) {
					if(PivotGridControl != null) {
						ApplyPagerModelSettingsToControl();
						ApplyFilterModelSettingsToControl();
						base.ApplyModel();
						ApplyPagerModelSettingsToControl();
					}
					OnModelApplied();
				}
			}
		}
		public override void SaveModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelSaving(args);
				if(!args.Cancel) {
					if(PivotGridControl != null) {
						SavePagerSettingsToModel();
						SaveFilterSettingsToModel();
						base.SaveModel();
					}
					OnModelSaved();
				}
			}
		}
		public override void Refresh() {
			base.Refresh();
			RefreshChart();
		}
		public override void BreakLinksToControls() {
			if(PivotGridControl != null) {
				PivotGridControl.Init -= new EventHandler(PivotGridControl_Init);
				PivotGridControl.Load -= new EventHandler(pivotGridControl_Load);
				PivotGridControl.FieldValueDisplayText -= new DevExpress.Web.ASPxPivotGrid.PivotFieldDisplayTextEventHandler(pivotGridControl_FieldValueDisplayText);
				PivotGridControl.CustomCellDisplayText -= new DevExpress.Web.ASPxPivotGrid.PivotCellDisplayTextEventHandler(pivotGridControl_CustomCellDisplayText);
				PivotGridControl.CustomSaveCallbackState -= new PivotGridCallbackStateEventHandler(PivotGridControl_CustomSaveCallbackState);
				PivotGridControl.CustomLoadCallbackState -= PivotGridControl_CustomLoadCallbackState;
				PivotGridControl.GridLayout -= new EventHandler(PivotGridControl_GridLayout);
				PivotGridControl.DataBound -= PivotGridControl_DataBound;
			}
			chartControl = null;
			if(controlContainer != null) {
				controlContainer.Dispose();
				controlContainer = null;
			}
			exporter = null;
			if(popupMenuManager != null) {
				popupMenuManager.Detach();
				popupMenuManager = null;
			}
			base.BreakLinksToControls();
			OnPrintableChanged();
		}
		public override void Dispose() {
			PrintableChanged = null;
			supportModelSavingEvent = null;
			base.Dispose();
		}
		public void UpdateChartVisibility(bool isVisible) {
			if(popupMenuManager != null) {
				this.popupMenuManager.UpdateMenuItemVisibility(isVisible);
			}
			ChartControl.Visible = isVisible;
			controlContainer.Container.Visible = isVisible;
			RefreshChart();
		}
		public override Boolean SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode) {
			return (dataAccessMode == CollectionSourceDataAccessMode.Client);
		}
		public new ASPxPivotGrid PivotGridControl {
			get { return (ASPxPivotGrid)base.PivotGridControl; }
		}
		public new WebChartControl ChartControl {
			get { return (WebChartControl)base.ChartControl; }
		}
		public List<ExportTarget> SupportedExportFormats {
			get {
				if(Printable == null) {
					return new List<ExportTarget>();
				}
				else {
					return new List<ExportTarget>(){
				ExportTarget.Csv,
				ExportTarget.Html,
				ExportTarget.Image,
				ExportTarget.Mht,
				ExportTarget.Pdf,
				ExportTarget.Rtf,
				ExportTarget.Text,
				ExportTarget.Xls,
				ExportTarget.Xlsx
				};
				}
			}
		}
		public IPrintable Printable {
			get { return exporter; }
		}
		public void OnExporting() { }
		public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
		private EventHandler<EventArgs> supportModelSavingEvent;
		event EventHandler<EventArgs> ISupportModelSaving.ModelSaving { add { supportModelSavingEvent += value; } remove { supportModelSavingEvent -= value; } }
		protected override void SetupPivotGridFieldToolTip(PivotGridFieldBase field, IModelToolTip toolTipModel) {
			DevExpress.Web.ASPxPivotGrid.PivotGridField pivotGridField = ((DevExpress.Web.ASPxPivotGrid.PivotGridField)field);
			if(toolTipModel != null) {
				DefaultHeaderTemplateWithToolTip templateWithToolTip = pivotGridField.HeaderTemplate as DefaultHeaderTemplateWithToolTip;
				if(templateWithToolTip == null) {
					templateWithToolTip = new DefaultHeaderTemplateWithToolTip();
					pivotGridField.HeaderTemplate = templateWithToolTip;
				}
				templateWithToolTip.ToolTip = toolTipModel.ToolTip;
			}
		}
		int ISupportPager.PageIndex {
			get {
				return PivotGridControl.OptionsPager.PageIndex;
			}
			set {
				PivotGridControl.OptionsPager.PageIndex = value;
			}
		}
		int ISupportPager.PageSize {
			get {
				return PivotGridControl.OptionsPager.RowsPerPage;
			}
			set {
				PivotGridControl.OptionsPager.RowsPerPage = value;
			}
		}
		bool ISupportFilter.FilterEnabled {
			get {
				return PivotGridControl.Prefilter.Enabled;
			}
			set {
				PivotGridControl.Prefilter.Enabled = value;
			}
		}
		string ISupportFilter.Filter {
			get {
				return PivotGridControl.Prefilter.CriteriaString;
			}
			set {
				PivotGridControl.Prefilter.CriteriaString = value;
			}
		}
		public ResizableControlContainer ChartControlContainer {
			get { return controlContainer; }
		}
		public event EventHandler<CancelEventArgs> PagerSettingsApplying;
		public event EventHandler<EventArgs> PagerSettingsApplied;
		public event EventHandler<CancelEventArgs> PagerSettingsSaving;
		public event EventHandler<CancelEventArgs> FilterSettingsSaving;
		public event EventHandler<CancelEventArgs> FilterSettingsApplying;
#if DebugTest
		public PivotGridPopupMenuManager DebugTest_PopupMenuManager {
			get { return popupMenuManager; }
		}
		public ResizableControlContainer DebugTest_ChartControlContainer {
			get { return controlContainer; }
		}
		public ASPxPivotGridExporter DebugTest_Exporter {
			get { return exporter; }
		}
#endif
	}
	public class DefaultHeaderTemplateWithToolTip : System.Web.UI.ITemplate {
		public void InstantiateIn(System.Web.UI.Control container) {
			PivotGridHeaderTemplateContainer c = (PivotGridHeaderTemplateContainer)container;
			PivotGridHeaderHtmlTable table = c.CreateHeader();
			c.Controls.Add(table);
			table.ToolTip = ToolTip; ;
		}
		public string ToolTip { get; set; }
	}
}
