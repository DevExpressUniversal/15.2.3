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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Menu;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPrinting;
using System.ComponentModel;
namespace DevExpress.ExpressApp.PivotGrid.Win {
	public enum ChartDesignerType { ChartWizard, ChartDesigner }
	public class PivotGridListEditor : PivotGridListEditorBase, IExportable, IDXPopupMenuHolder, ISupportFilter, IContextMenuTarget {
		[System.ComponentModel.DefaultValue(ChartDesignerType.ChartDesigner)]
		public static ChartDesignerType ChartDesignerType = ChartDesignerType.ChartDesigner;
		private XafLayoutControl layoutControl;
		private LayoutControlItem chartLayoutItem;
		private ToolStripMenuItem hideShowChartMenuItem;
		private string chartDefaultSettings;
		private ActionsDXPopupMenu popupMenu;
		private void miWizard_Click(object sender, EventArgs e) {
			ShowWizard();
		}
		private void miClear_Click(object sender, EventArgs e) {
			ResetSettings();
		}
		private void miChart_Click(object sender, EventArgs e) {
			if(chartLayoutItem != null) {
				PivotSettings.ShowChart = !PivotSettings.ShowChart;
				if(chartLayoutItem.Visibility != LayoutVisibility.Always) {
					chartLayoutItem.Visibility = LayoutVisibility.Always;
					hideShowChartMenuItem.Text = CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "HideChart");
					hideShowChartMenuItem.Image = ImageLoader.Instance.GetImageInfo("Action_Hide_Chart").Image;
					RefreshChart();
				}
				else {
					chartLayoutItem.Visibility = LayoutVisibility.OnlyInCustomization;
					hideShowChartMenuItem.Text = CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ShowChart");
					hideShowChartMenuItem.Image = ImageLoader.Instance.GetImageInfo("Action_View_Chart").Image;
				}
				UpdateSplitterVisibility(chartLayoutItem.Visibility);
			}
		}
		private void UpdateSplitterVisibility(LayoutVisibility visibility) {
			SplitterItem splitterItem = null;
			foreach(BaseLayoutItem item in layoutControl.Items) {
				splitterItem = item as SplitterItem;
				if(splitterItem != null) {
					break;
				}
			}
			if(splitterItem != null) {
				splitterItem.Visibility = visibility;
			}
		}
		private void ShowWizard() {
			using(XafPivotGridDesignerForm editor = new XafPivotGridDesignerForm(UserLookAndFeel.Default)) {
				if(PivotGridControl != null) {
					editor.InitEditingObject(PivotGridControl);
					editor.ShowDialog();
					PivotGridControl.Site = null;
					SavePivotSettings();
				}
			}
		}
		private void RefreshChart() {
			ChartControl.DataSource = PivotGridControl;
			ChartControl.SeriesDataMember = "Series";
			ChartControl.SeriesTemplate.ArgumentDataMember = "Arguments";
			ChartControl.SeriesTemplate.ValueDataMembers.AddRange(new string[] { "Values" });
			ChartControl.RefreshData();
		}
		private void miChartWizard_Click(object sender, EventArgs e) {
			if(ChartDesignerType == ChartDesignerType.ChartWizard) {
				ChartWizard wizard = new ChartWizard(ChartControl);
				wizard.ShowDialog();
			}
			else {
				XtraCharts.Designer.ChartDesigner designer = new XtraCharts.Designer.ChartDesigner(ChartControl);
				designer.ShowDialog();
			}
		}
		private DXMenuItem CreateChartOptionsSubMenu() {
			DXSubMenuItem miChartOptions = new DXSubMenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ChartOptions"));
			miChartOptions.Image = ImageLoader.Instance.GetImageInfo("Action_Chart_Options").Image;
			miChartOptions.Enabled = PivotSettings.ShowChart;
			DXMenuCheckItem miChartDataVertical = new DXMenuCheckItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ChartDataVertical"), PivotSettings.ChartDataVertical, ImageLoader.Instance.GetImageInfo("Action_ChartDataVertical").Image, null);
			miChartDataVertical.Click += delegate(object s, EventArgs args) {
				PivotSettings.ChartDataVertical = !PivotSettings.ChartDataVertical;
				PivotGridControl.OptionsChartDataSource.ProvideDataByColumns = PivotSettings.ChartDataVertical;
			};
			miChartOptions.Items.Add(miChartDataVertical);
			DXMenuCheckItem miShowRowTotals = new DXMenuCheckItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ShowRowTotals"), PivotSettings.ShowRowTotals, ImageLoader.Instance.GetImageInfo("Action_Totals_Row").Image, null);
			miShowRowTotals.Click += delegate(object s, EventArgs args) {
				PivotSettings.ShowRowTotals = !PivotSettings.ShowRowTotals;
				PivotGridControl.OptionsChartDataSource.ProvideRowTotals = PivotSettings.ShowRowTotals;
			};
			miChartOptions.Items.Add(miShowRowTotals);
			DXMenuCheckItem miShowRowGrandTotals = new DXMenuCheckItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ShowRowGrandTotals"), PivotSettings.ShowRowGrandTotals, ImageLoader.Instance.GetImageInfo("Action_Grand_Totals_Row").Image, null);
			miShowRowGrandTotals.Click += delegate(object s, EventArgs args) {
				PivotSettings.ShowRowGrandTotals = !PivotSettings.ShowRowGrandTotals;
				PivotGridControl.OptionsChartDataSource.ProvideRowGrandTotals = PivotSettings.ShowRowGrandTotals;
			};
			miChartOptions.Items.Add(miShowRowGrandTotals);
			DXMenuCheckItem miShowColumnTotals = new DXMenuCheckItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ShowColumnTotals"), PivotSettings.ShowColumnTotals, ImageLoader.Instance.GetImageInfo("Action_Totals_Column").Image, null);
			miShowColumnTotals.Click += delegate(object s, EventArgs args) {
				PivotSettings.ShowColumnTotals = !PivotSettings.ShowColumnTotals;
				PivotGridControl.OptionsChartDataSource.ProvideColumnTotals = PivotSettings.ShowColumnTotals;
			};
			miChartOptions.Items.Add(miShowColumnTotals);
			DXMenuCheckItem miShowColumnGrandTotals = new DXMenuCheckItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ShowColumnGrandTotals"), PivotSettings.ShowColumnGrandTotals, ImageLoader.Instance.GetImageInfo("Action_Grand_Totals_Column").Image, null);
			miShowColumnGrandTotals.Click += delegate(object s, EventArgs args) {
				PivotSettings.ShowColumnGrandTotals = !PivotSettings.ShowColumnGrandTotals;
				PivotGridControl.OptionsChartDataSource.ProvideColumnGrandTotals = PivotSettings.ShowColumnGrandTotals;
			};
			miChartOptions.Items.Add(miShowColumnGrandTotals);
			return miChartOptions;
		}
		private void pivotGridControl_PopupMenuShowing(object sender, DevExpress.XtraPivotGrid.PopupMenuShowingEventArgs e) {
			if(e.MenuType == PivotGridMenuType.HeaderArea) {
				if(PivotSettings.CustomizationEnabled) {
					DXMenuItem miWizard = new DXMenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "InvokeWizard"), miWizard_Click, ImageLoader.Instance.GetImageInfo("Action_Show_PivotGrid_Designer").Image);
					miWizard.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
					e.Menu.Items.Add(miWizard);
					DXMenuItem miClear = new DXMenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ClearSettings"), miClear_Click, ImageLoader.Instance.GetImageInfo("Action_Clear_Settings").Image);
					miClear.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
					e.Menu.Items.Add(miClear);
				}
				string showChartMenuCaptionId = PivotSettings.ShowChart ? "HideChart" : "ShowChart";
				string showChartMenuImageId = PivotSettings.ShowChart ? "Action_Hide_Chart" : "Action_View_Chart";
				DXMenuItem miChart = new DXMenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, showChartMenuCaptionId), miChart_Click, ImageLoader.Instance.GetImageInfo(showChartMenuImageId).Image);
				miChart.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
				e.Menu.Items.Add(miChart);
				e.Menu.Items.Add(CreateChartOptionsSubMenu());
			}
		}
		private void pivotGridControl_FieldPropertyChanged(object sender, PivotFieldPropertyChangedEventArgs e) {
			if(e.PropertyName == PivotFieldPropertyName.FieldName) {
				IMemberInfo memberInfo = Model.ModelClass.TypeInfo.FindMember(e.Field.FieldName);
				if(memberInfo != null) {
					SetupPivotGridField(e.Field, memberInfo);
				}
			}
		}
		private void pivotGridControl_CustomCellDisplayText(object sender, PivotCellDisplayTextEventArgs e) {
			string displayText = GetDisplayText(e.DataField, e.Value);
			if(!string.IsNullOrEmpty(displayText)) {
				e.DisplayText = displayText;
			}
		}
		private void pivotGridControl_FieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e) {
			string displayText = GetDisplayText(e.Field, e.Value);
			if(!string.IsNullOrEmpty(displayText)) {
				e.DisplayText = displayText;
			}
		}
		private void LoadLayoutSettings() {
			if(!string.IsNullOrEmpty(PivotSettings.LayoutSettings) && layoutControl != null) {
				MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(PivotSettings.LayoutSettings));
				layoutControl.RestoreLayoutFromStream(ms);
			}
		}
		private void LoadChartSettings() {
			if(!string.IsNullOrEmpty(PivotSettings.LayoutSettings)) {
				MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(PivotSettings.ChartSettings));
				ChartControl.LoadFromStream(ms);
			}
		}
		private void SaveLayoutSettings() {
			if(layoutControl != null) {
				MemoryStream ms = new MemoryStream();
				layoutControl.SaveLayoutToStream(ms);
				PivotSettings.LayoutSettings = Encoding.UTF8.GetString(ms.ToArray());
			}
		}
		private void SaveChartSettings() {
			MemoryStream ms = new MemoryStream();
			ChartControl.SaveToStream(ms);
			PivotSettings.ChartSettings = Encoding.UTF8.GetString(ms.ToArray());
		}
		protected override void LoadPivotSettings(string settings) {
			if(!string.IsNullOrEmpty(settings)) {
				InitializeOptions(PivotGridControl.OptionsLayout);
				MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(settings));
				PivotGridControl.RestoreLayoutFromStream(ms);
				PivotGridControl.OptionsCustomization.AllowPrefilter = false;
			}
		}
		protected override string GetPivotGridSettings() {
			MemoryStream ms = new MemoryStream();
			PivotGridControl.SaveLayoutToStream(ms);
			return Encoding.UTF8.GetString(ms.ToArray());
		}
		protected override PivotGridFieldBase CreatePivotGridField(string propertyName, PivotArea pivotArea) {
			return new PivotGridField(propertyName, pivotArea);
		}
		protected void OnPrintableChanged() {
			if(PrintableChanged != null) {
				PrintableChanged(this, new PrintableChangedEventArgs(Printable));
			}
		}
		protected override object CreateControlsCore() {
			this.pivotGridControl = new XafPivotGridControl();
			PivotGridControl.FieldPropertyChanged += new PivotFieldPropertyChangedEventHandler(pivotGridControl_FieldPropertyChanged);
			PivotGridControl.FieldValueDisplayText += new PivotFieldDisplayTextEventHandler(pivotGridControl_FieldValueDisplayText);
			PivotGridControl.CustomCellDisplayText += new PivotCellDisplayTextEventHandler(pivotGridControl_CustomCellDisplayText);
			PivotGridControl.Name = "PivotGrid";
			InitializeOptions(PivotGridControl.OptionsLayout);
			InitializePivotGridData(((IPivotGridViewInfoDataOwner)PivotGridControl).DataViewInfo);
			PivotGridControl.PopupMenuShowing += new DevExpress.XtraPivotGrid.PopupMenuShowingEventHandler(pivotGridControl_PopupMenuShowing);
			this.chartControl = new ChartControl();
			ChartControl.Name = "Chart";
			MemoryStream ms = new MemoryStream();
			ChartControl.SaveToStream(ms);
			chartDefaultSettings = Encoding.UTF8.GetString(ms.ToArray());
			ChartControl.ContextMenuStrip = new ContextMenuStrip();
			ChartControl.ContextMenuStrip.Text = "Tasks";
			if(PivotSettings.CustomizationEnabled) {
				ToolStripMenuItem chartWizardMenuItem = new ToolStripMenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "InvokeWizard"), ImageLoader.Instance.GetImageInfo("Action_Chart_ShowDesigner").Image);
				chartWizardMenuItem.ShortcutKeys = Keys.Control | Keys.W;
				chartWizardMenuItem.Click += miChartWizard_Click;
				ToolStripMenuItem clearChartSettingsMenuItem = new ToolStripMenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ClearSettings"), ImageLoader.Instance.GetImageInfo("Action_Clear_Settings").Image);
				clearChartSettingsMenuItem.ShortcutKeys = Keys.Control | Keys.C;
				clearChartSettingsMenuItem.Click += delegate(object s, EventArgs args) {
					PivotSettings.ChartSettings = null;
					MemoryStream chartDefaultsMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(chartDefaultSettings));
					ChartControl.LoadFromStream(chartDefaultsMemoryStream);
					RefreshChart();
				};
				ChartControl.ContextMenuStrip.Items.AddRange(new ToolStripMenuItem[] { chartWizardMenuItem, clearChartSettingsMenuItem });
			}
			string showChartMenuCaptionId = PivotSettings.ShowChart ? "HideChart" : "ShowChart";
			hideShowChartMenuItem = new ToolStripMenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, showChartMenuCaptionId), ImageLoader.Instance.GetImageInfo("Action_Hide_Chart").Image);
			hideShowChartMenuItem.ShortcutKeys = Keys.Control | Keys.H;
			hideShowChartMenuItem.Click += miChart_Click;
			ChartControl.ContextMenuStrip.Items.Add(hideShowChartMenuItem);
			LoadChartSettings();
			layoutControl = new XafLayoutControl();
			layoutControl.Controls.Add(PivotGridControl);
			layoutControl.Controls.Add(ChartControl);
			layoutControl.OptionsView.PaddingSpacingMode = PaddingMode.MSGuidelines;
			layoutControl.XafLayoutConstants.ItemToBorderHorizontalDistance = 0;
			layoutControl.XafLayoutConstants.ItemToBorderVerticalDistance = 0;
			layoutControl.XafLayoutConstants.ItemToItemHorizontalDistance = 0;
			layoutControl.XafLayoutConstants.ItemToItemVerticalDistance = 0;
			if(string.IsNullOrEmpty(PivotSettings.LayoutSettings)) {
				layoutControl.BeginUpdate();
				layoutControl.Root.BeginUpdate();
				LayoutGroup mainGroup = layoutControl.Root.AddGroup("Main");
				mainGroup.TextVisible = false;
				mainGroup.GroupBordersVisible = false;
				LayoutControlItem pivotGridLayoutItem = new LayoutControlItem(layoutControl, PivotGridControl);
				pivotGridLayoutItem.BeginInit();
				pivotGridLayoutItem.Name = "PivotGrid";
				pivotGridLayoutItem.TextVisible = false;
				pivotGridLayoutItem.EndInit();
				mainGroup.AddItem(pivotGridLayoutItem);
				SplitterItem splitterItem = new SplitterItem();
				splitterItem.Visibility = PivotSettings.ShowChart ? LayoutVisibility.Always : LayoutVisibility.OnlyInCustomization;
				mainGroup.AddItem(splitterItem);
				chartLayoutItem = new LayoutControlItem(layoutControl, ChartControl);
				chartLayoutItem.BeginInit();
				chartLayoutItem.Name = "Chart";
				chartLayoutItem.TextVisible = false;
				chartLayoutItem.Visibility = PivotSettings.ShowChart ? LayoutVisibility.Always : LayoutVisibility.OnlyInCustomization;
				chartLayoutItem.EndInit();
				mainGroup.AddItem(chartLayoutItem);
				layoutControl.BestFit();
				layoutControl.Root.EndUpdate();
				layoutControl.EndUpdate();
			}
			else {
				LoadLayoutSettings();
				foreach(BaseLayoutItem item in layoutControl.Items) {
					if(item is LayoutControlItem && ((LayoutControlItem)item).Control is ChartControl) {
						chartLayoutItem = (LayoutControlItem)item;
						break;
					}
				}
			}
			OnPrintableChanged();
			return layoutControl;
		}
		public PivotGridListEditor(IModelListView model)
			: base(model) {
			popupMenu = new ActionsDXPopupMenu();
		}
		protected override bool TryUpdateControlDataSource() {
			if(PivotGridControl != null) {
				IList sourceList = null;
				if(List is IListServer) {
					throw new Exception("The PivotGridListEditor doesn't support Server Mode and so cannot use an IListServer object as the data source.");
				}
				else {
					sourceList = List;
				}
				PivotGridControl.DataSource = sourceList;
				if(chartLayoutItem != null && chartLayoutItem.Visibility == LayoutVisibility.Always) {
					RefreshChart();
				}
				return true;
			}
			return false;
		}
		public override void Refresh() {
			UpdateControlDataSource();
		}
		public override void ApplyModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelApplying(args);
				if(!args.Cancel) {
					base.ApplyModel();
					if(PivotGridControl != null) {
						PivotGridControl.Prefilter.CriteriaString = Model.Filter;
						PivotGridControl.Prefilter.Enabled = Model.FilterEnabled;
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
					base.SaveModel();
					if(PivotGridControl != null) {
						Model.Filter = PivotGridControl.Prefilter.CriteriaString;
						Model.FilterEnabled = PivotGridControl.Prefilter.Enabled;
					}
					SaveLayoutSettings();
					if(chartLayoutItem != null) {
						SaveChartSettings();
						PivotSettings.ShowChart = chartLayoutItem.Visibility == LayoutVisibility.Always;
					}
					OnModelSaved();
				}
			}
		}
		public override void Dispose() {
			if(PivotGridControl != null) {
				PivotGridControl.PopupMenuShowing -= new DevExpress.XtraPivotGrid.PopupMenuShowingEventHandler(pivotGridControl_PopupMenuShowing);
				PivotGridControl.FieldPropertyChanged -= new PivotFieldPropertyChangedEventHandler(pivotGridControl_FieldPropertyChanged);
				PivotGridControl.FieldValueDisplayText -= new PivotFieldDisplayTextEventHandler(pivotGridControl_FieldValueDisplayText);
				PivotGridControl.CustomCellDisplayText -= new PivotCellDisplayTextEventHandler(pivotGridControl_CustomCellDisplayText);
			}
			popupMenu.Dispose();
			popupMenu = null;
			base.Dispose();
		}
		public override void BreakLinksToControls() {
			this.layoutControl = null;
			if(PivotGridControl != null) {
				PivotGridControl.FieldPropertyChanged -= new PivotFieldPropertyChangedEventHandler(pivotGridControl_FieldPropertyChanged);
				PivotGridControl.FieldValueDisplayText -= new PivotFieldDisplayTextEventHandler(pivotGridControl_FieldValueDisplayText);
				PivotGridControl.CustomCellDisplayText -= new PivotCellDisplayTextEventHandler(pivotGridControl_CustomCellDisplayText);
			}
			if(ChartControl != null) {
				ChartControl.ContextMenuStrip.Items.Remove(hideShowChartMenuItem);
				ChartControl.ContextMenuStrip = null;
				hideShowChartMenuItem = null;
			}
			base.BreakLinksToControls();
			OnPrintableChanged();
		}
		public override Boolean SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode) {
			return (dataAccessMode == CollectionSourceDataAccessMode.Client);
		}
		public override IContextMenuTemplate ContextMenuTemplate {
			get { return popupMenu; }
		}
		public new PivotGridControl PivotGridControl {
			get { return (PivotGridControl)base.PivotGridControl; }
		}
		public new ChartControl ChartControl {
			get { return (ChartControl)base.ChartControl; }
		}
		#region IDXPopupMenuHolder Members
		public Control PopupSite {
			get { return PivotGridControl; }
		}
		public virtual bool CanShowPopupMenu(Point position) {
			PivotGridHitTest hitTest = PivotGridControl.CalcHitInfo(PivotGridControl.PointToClient(position)).HitTest;
			return
				 ((hitTest == PivotGridHitTest.Cell)
				 || (hitTest == PivotGridHitTest.None));
		}
		public void SetMenuManager(IDXMenuManager manager) {
			if(PivotGridControl != null) {
				PivotGridControl.MenuManager = manager;
			}
		}
		#endregion
		#region IContextMenuTarget Members
		void IContextMenuTarget.SetMenuManager(IDXMenuManager menuManager) {
			if(PivotGridControl == null) {
				throw new InvalidOperationException("Cannot set the 'MenuManager' property because the 'PivotGridListEditor.PivotGridControl' property is null");
			}
			if(menuManager == null && PivotGridControl.IsDisposed) {
				return;
			}
			PivotGridControl.MenuManager = menuManager;
		}
		bool IContextMenuTarget.CanShowContextMenu(Point position) {
			return CanShowPopupMenu(position);
		}
		Control IContextMenuTarget.ContextMenuSite {
			get { return PivotGridControl; }
		}
		bool IContextMenuTarget.ContextMenuEnabled {
			get { return true; }
		}
		event EventHandler IContextMenuTarget.ContextMenuEnabledChanged {
			add { }
			remove { }
		}
		#endregion
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
			get {
				if(chartLayoutItem != null && chartLayoutItem.Visibility == LayoutVisibility.Always) {
					return layoutControl;
				}
				return PivotGridControl;
			} 
		}
		public void OnExporting() { }
		public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
		protected override void SetupPivotGridFieldToolTip(PivotGridFieldBase field, IModelToolTip toolTipModel) {
			PivotGridField pivotGridField = (PivotGridField)field;
			if((toolTipModel != null) && string.IsNullOrEmpty(pivotGridField.ToolTips.HeaderText)) {
				pivotGridField.ToolTips.HeaderText = toolTipModel.ToolTip;
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
	}
}
