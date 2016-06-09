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
using System.Windows.Forms;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Designer.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraTab;
namespace DevExpress.XtraCharts.Designer {
	public partial class ChartDesignerController : Component {
		object selectedElement;
		CommandManager commandManager;
		DesignerChartModel chartModel;
		IChartContainer chartContainer;
		Chart chart;
		ChartDesignControl designControl;
		ChartPreviewControl previewControl;
		ChartPropertyGridControl propertyGridControl;
		ChartStructureControl chartStructureControl;
		ChartElementsOptionsControl optionsControl;
		ChartActionsControl actionsControl;
		ChartDataControl dataControl;
		SeriesPointsGridControl pointsGridControl;
		XtraTabPage tabData;
		XtraTabPage tabOptions;
		XtraTabPage tabProperties;
		bool isDesignTime;
		internal Chart Chart { get { return chart; } }
		internal bool ChartChanged { get { return commandManager.CanUndo; } }
		internal DesignerChartModel ChartModel { get { return chartModel; } }
		public IChartContainer ChartContainer {
			get { return chartContainer; }
			set {
				chartContainer = value;
				OnChartChanged();
			}
		}
		public ChartPreviewControl PreviewControl {
			get { return previewControl; }
			set {
				if (previewControl != null)
					previewControl.HotKeyPressed -= OnHotKeyPressed;
				previewControl = value;
				OnPreviewControlChanged();
			}
		}
		public ChartPropertyGridControl PropertyGridControl {
			get { return propertyGridControl; }
			set {
				propertyGridControl = value;
				OnPropertyGridChanged();
			}
		}
		public ChartStructureControl ChartStructureControl {
			get { return chartStructureControl; }
			set {
				if (chartStructureControl != null)
					chartStructureControl.HotKeyPressed -= OnHotKeyPressed;
				chartStructureControl = value;
				OnChartStructureControlChanged();
			}
		}
		public ChartElementsOptionsControl ChartOptionsControl {
			get { return optionsControl; }
			set {
				optionsControl = value;
				OnChartOptionsControlChanged();
			}
		}
		public ChartActionsControl ActionsControl {
			get { return actionsControl; }
			set {
				OnActionsControlChanged(value);
				actionsControl = value;
			}
		}
		public ChartDataControl DataControl {
			get { return dataControl; }
			set {
				dataControl = value;
				OnDataControlChanged();
			}
		}
		public SeriesPointsGridControl PointsGridControl {
			get { return pointsGridControl; }
			set { pointsGridControl = value; }
		}
		public ChartDesignerController() {
			InitializeComponent();
			Initialize();
		}
		public ChartDesignerController(IContainer container) {
			container.Add(this);
			InitializeComponent();
			Initialize();
		}
		void Initialize() {
			commandManager = new CommandManager();
			commandManager.CommandExecuted += OnCommandExecuted;
		}
		void OnChartChanged() {
			if (chartContainer != null) {
				Chart originalChart = chartContainer.Chart;
				IChartDataProvider dataProvider = chartContainer.DataProvider;
				if (dataProvider != null)
					designControl = new ChartDesignControl(chartContainer.ControlType, dataProvider.ParentDataSource, GetActualDataContext(chartContainer.ServiceProvider, dataProvider.DataContext));
				else
					designControl = new ChartDesignControl(chartContainer.ControlType, null, GetActualDataContext(chartContainer.ServiceProvider, null));
				designControl.CanDisposeItems = false;
				chart = new Chart(designControl);
				designControl.Chart = chart;
				chart.Assign(originalChart);
				chartModel = new DesignerChartModel(commandManager, chart);
				designControl.SelectionMode = ElementSelectionMode.Single;
				designControl.Dock = DockStyle.Fill;
				designControl.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
				designControl.ObjectSelected += DesignControl_ObjectSelected;
				designControl.ObjectHotTracked += DesignControl_ObjectHotTracked;
				designControl.MouseLeave += DesignControl_MouseLeave;
				designControl.SetServiceProvider(chartContainer);
				if (chartStructureControl != null)
					chartStructureControl.ChartModel = chartModel;
				if (actionsControl != null)
					actionsControl.ChartModel = chartModel;
				if (dataControl != null) {
					dataControl.IsDesignTime = isDesignTime;
					dataControl.ServiceProvider = chartContainer;
					dataControl.ChartModel = chartModel;
				}
				if (propertyGridControl != null)
					propertyGridControl.ServiceProvider = chartContainer;
				SelectedModelChanged(chartModel);
				if (previewControl != null) {
					previewControl.Controls.Add(designControl);
					designControl.Chart.SetObjectSelection(chart, true);
				}
			}
		}
		void OnPreviewControlChanged() {
			if (previewControl != null) {
				if (designControl != null)
					previewControl.Controls.Add(designControl);
				previewControl.HotKeyPressed += OnHotKeyPressed;
			}
		}
		void OnPropertyGridChanged() {
			if (propertyGridControl != null) {
				propertyGridControl.SelectedObject = chartModel;
				propertyGridControl.ServiceProvider = chartContainer;
			}
		}
		void OnChartStructureControlChanged() {
			if (chartStructureControl != null) {
				chartStructureControl.ChartModel = chartModel;
				chartStructureControl.SelectionChanged += ChartStructureControl_SelectionChanged;
				chartStructureControl.HighlightedItemChanged += ChartStructureControl_HighlightedItemChanged;
				chartStructureControl.HotKeyPressed += OnHotKeyPressed;
			}
		}
		void OnChartOptionsControlChanged() {
			if (optionsControl != null)
				optionsControl.SelectedModel = chartModel;
		}
		void OnActionsControlChanged(ChartActionsControl newControl) {
			if (actionsControl != null)
				actionsControl.ActionButtonPressed -= ActionsControlButtonPressed;
			if (newControl != null) {
				newControl.ChartModel = chartModel;
				newControl.ActionButtonPressed += ActionsControlButtonPressed;
			}
		}
		void OnDataControlChanged() {
			if (dataControl != null) {
				dataControl.IsDesignTime = isDesignTime;
				dataControl.ServiceProvider = chartContainer;
				dataControl.ChartModel = chartModel;
			}
		}
		void ActionsControlButtonPressed(object sender, ActionButtonPressedEventArgs e) {
			switch (e.ButtonType) {
				case ActionButtonType.Undo:
					Undo();
					break;
				case ActionButtonType.Redo:
					Redo();
					break;
			}
		}
		void OnHotKeyPressed(object sender, HotKeyPressedEventArgs e) {
			if (e.KeyData == Keys.Delete)
				RemoveSelectedElement();
		}
		DataContext GetActualDataContext(IServiceProvider serviceProvider, DataContext defaultContext) {
			if (serviceProvider != null) {
				IDataContextService service = serviceProvider.GetService(typeof(IDataContextService)) as IDataContextService;
				if (service != null)
					return service.CreateDataContext(new DataContextOptions(true, true), false);
			}
			return defaultContext;
		}
		void DesignControl_MouseLeave(object sender, EventArgs e) {
			if (chartStructureControl != null)
				chartStructureControl.HighlightedElement = null;
		}
		void DesignControl_ObjectHotTracked(object sender, HotTrackEventArgs e) {
			object hotElement = e.Object;
			if (hotElement == designControl)
				hotElement = designControl.Chart;
			if (chartStructureControl != null)
				chartStructureControl.HighlightedElement = hotElement;
		}
		void DesignControl_ObjectSelected(object sender, HotTrackEventArgs e) {
			selectedElement = e.Object;
			if (selectedElement == designControl)
				selectedElement = designControl.Chart;
			DesignerChartElementModelBase selectedModel = chartModel.FindElementModel(selectedElement);
			if (chartStructureControl != null)
				chartStructureControl.SelectedElement = selectedElement;
			SelectedModelChanged(selectedModel);
		}
		void ChartStructureControl_SelectionChanged(ChartTreeSelectionChangedEventArgs e) {
			DesignerChartElementModelBase selectedModel = e.SelectedChartElementModel;
			designControl.Chart.ClearSelection(true);
			if (selectedModel != null)
				designControl.Chart.SetObjectSelection(selectedModel.ChartElement, true);
			SelectedModelChanged(selectedModel);
		}
		void SelectedModelChanged(DesignerChartElementModelBase selectedModel) {
			if (propertyGridControl != null)
				propertyGridControl.SelectedObject = selectedModel;
			if (optionsControl != null)
				optionsControl.SelectedModel = selectedModel;
			if (actionsControl != null)
				actionsControl.SelectedModel = selectedModel;
			if (dataControl != null)
				dataControl.SelectedModel = selectedModel;
			if (pointsGridControl != null)
				pointsGridControl.SelectedModel = selectedModel as SeriesPointCollectionModel;
			UpdateTabs(selectedModel);
		}
		void UpdateTabs(DesignerChartElementModelBase selectedModel) {
			bool showOptions = selectedModel != null && !(selectedModel is ChartCollectionBaseModel);
			if (tabOptions != null)
				tabOptions.PageVisible = showOptions;
			if (tabProperties != null)
				tabProperties.PageVisible = showOptions;
			if (tabData != null) {
				tabData.Controls.Clear();
				tabData.PageVisible = dataControl != null && selectedModel != null && selectedModel.IsSupportsDataControl(isDesignTime);
				if (tabData.PageVisible) {
					if (selectedModel is SeriesPointCollectionModel)
						tabData.Controls.Add(pointsGridControl);
					else
						tabData.Controls.Add(dataControl);
				}
			}
		}
		void ChartStructureControl_HighlightedItemChanged(ChartTreeHighlightedItemChangedEventArgs e) {
			if (e.HighlightedChartElementModel != null) {
				IHitTest hitElement = e.HighlightedChartElementModel.ChartElement as IHitTest;
				designControl.Chart.HotHitElement(hitElement);
			}
			else
				designControl.Chart.HotHitElement(null);
		}
		void OnCommandExecuted(CommandExecutedEventArgs e) {
			chartModel.Update();
			string stringParameter = e.CommandParameter as string;
			if (chartStructureControl != null)
				chartStructureControl.UpdateTree();
			if (e.ObjectToSelect != null) {
				if (chartStructureControl != null)
					chartStructureControl.SelectedElement = e.ObjectToSelect;
				else
					designControl.Chart.SetObjectSelection(e.ObjectToSelect, true);
			}
			if (propertyGridControl != null)
				propertyGridControl.UpdateProperties();
			if (optionsControl != null)
				optionsControl.UpdateOptions(e.CommandParameter);
			if (pointsGridControl != null && e.HistoryItem != null && e.HistoryItem.Command.CanUpdatePointsGrid)
				pointsGridControl.UpdateData();
			if (actionsControl != null)
				actionsControl.UpdateControls();
			if (dataControl != null)
				dataControl.UpdateState();
		}
		object GetActualSelectedElement() {
			return chartStructureControl != null ? chartStructureControl.SelectedElement : selectedElement;
		}
		void RemoveSelectedElement() {
			object elementToRemove = GetActualSelectedElement();
			if (elementToRemove != null) {
				DesignerChartElementModelBase model = chartModel.FindElementModel(elementToRemove);
				if (model != null) {
					if (model.ParentCollection != null)
						model.ParentCollection.DeleteElement(model);
					else if (model is ISupportModelVisibility)
						((ISupportModelVisibility)model).Visible = false;
				}
			}
		}
		internal void SetTabs(XtraTabPage tabData, XtraTabPage tabProperties, XtraTabPage tabOptions) {
			this.tabData = tabData;
			this.tabProperties = tabProperties;
			this.tabOptions = tabOptions;
			UpdateTabs(chartModel);
		}
		internal void SetIsDesignTime(bool isDesignTime) {
			this.isDesignTime = isDesignTime;
			if (dataControl != null)
				dataControl.IsDesignTime = isDesignTime;
			UpdateTabs(chartModel);
		}
		public void Undo() {
			commandManager.Undo();
		}
		public void Redo() {
			commandManager.Redo();
		}
		public void ApplyChanges() {
			if (chartContainer != null) {
				chartContainer.Chart.Assign(chart);
				chartContainer.Changed();
			}
		}
	}
}
namespace DevExpress.XtraCharts.Designer.Native {
	public class HotKeyPressedEventArgs : EventArgs {
		readonly Keys keyData;
		public Keys KeyData { get { return keyData; } }
		public HotKeyPressedEventArgs(Keys keyData) {
			this.keyData = keyData;
		}
	}
	public delegate void HotKeyPressedEventHandler(object sender, HotKeyPressedEventArgs e);
}
