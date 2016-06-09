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

using System.Collections.Generic;
using System.Windows.Input;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public sealed partial class WpfChartModel : ChartModelElement {
		const string BindingPathToSelectedModel = "SelectedModel";
		readonly List<ChartCommandBase> commandsRepository = new List<ChartCommandBase>();
		readonly List<RibbonPageCategoryViewModel> categories = new List<RibbonPageCategoryViewModel>();
		readonly RemoveElementCommand deleteCommand;
		readonly ChartDataEditorPointCollectionModel dataEditorPointCollectionModel;
		readonly object predefinedDataSource;
		WpfChartDiagramModel diagramModel;
		WpfChartTitleCollectionModel titleCollectionModel;
		CommandManager commandManager;
		object selectedObject;
		WpfChartLegendModel legendModel;
		ChartModelElement selectedModel;
		string selectedPageName = string.Empty;
		List<ValueLevelComboBoxPresentation> selectedIndicatorAcceptedValueLevels;
		ICommand DeleteCommand {
			get { return deleteCommand; }
		}
		public override IEnumerable<ChartModelElement> Children {
			get { return new ChartModelElement[] { LegendModel, DiagramModel, TitleCollectionModel }; }
		}
		public CommandManager CommandManager {
			get { return commandManager; }
		}
		public ChartControl Chart {
			get { return (ChartControl)ChartElement; }
		}
		public WpfChartDiagramModel DiagramModel {
			get { return diagramModel; }
			private set {
				if (diagramModel != value) {
					diagramModel = value;
					OnPropertyChanged("DiagramModel");
				}
			}
		}
		public WpfChartLegendModel LegendModel {
			get { return legendModel; }
			private set {
				if (legendModel != value) {
					legendModel = value;
					OnPropertyChanged("LegendModel");
				}
			}
		}
		public WpfChartTitleCollectionModel TitleCollectionModel {
			get { return titleCollectionModel; }
			private set {
				if (titleCollectionModel != value) {
					titleCollectionModel = value;
					OnPropertyChanged("TitleCollectionModel");
				}
			}
		}
		public object SelectedObject {
			get { return selectedObject; }
			set {
				if (selectedObject != value && value != null) {
					selectedObject = value;
					OnPropertyChanged("SelectedObject");
					ChartModelElement foundedElement = PerformFullSearch(selectedObject);
					SelectedModel = (foundedElement == null) ? null : foundedElement.SelectionOverride;
					UpdateCommandsCanExecute();
				}
			}
		}
		public ChartModelElement SelectedModel {
			get { return selectedModel; }
			set {
				if (selectedModel != value) {
					selectedModel = value;
					object selectedObject = (selectedModel == null) ? null : selectedModel.ChartElement;
					if (selectedObject != null)
						SelectedObject = selectedObject;
					OnSelectedModelChanged();
					OnPropertyChanged("SelectedModel");
				}
			}
		}
		public ChartDataEditorPointCollectionModel DataEditorModel {
			get {
				return dataEditorPointCollectionModel;
			}
		}
		public object PredefinedDataSource { get { return predefinedDataSource; } }
		public object DataSource {
			get { return Chart.DataSource; }
			set {
				if (Chart.DataSource != value) {
					Chart.DataSource = value;
					OnPropertyChanged("DataSource");
				}
			}
		}
		public string SelectedPageName {
			get { return selectedPageName; }
			set {
				if (selectedPageName != value && !string.IsNullOrEmpty(value)) {
					selectedPageName = value;
					if (!string.IsNullOrEmpty(selectedPageName))
						OnPropertyChanged("SelectedPageName");
				}
			}
		}
		public List<ValueLevelComboBoxPresentation> SelectedIndicatorAcceptedValueLevels {
			get { return selectedIndicatorAcceptedValueLevels; }
			set {
				if (selectedIndicatorAcceptedValueLevels != value) {
					selectedIndicatorAcceptedValueLevels = value;
					OnPropertyChanged("SelectedIndicatorAcceptedValueLevels");
				}
			}
		}
		public WpfChartModel(ChartControl chart, CommandManager commandManager)
			: base(null, chart) {
			this.SelectedObject = chart;
			this.commandManager = commandManager;
			this.dataEditorPointCollectionModel = new ChartDataEditorPointCollectionModel(this);
			this.titleCollectionModel = new WpfChartTitleCollectionModel(this, chart.Titles);
			deleteCommand = new RemoveElementCommand(this);
			predefinedDataSource = chart.DataSource;
			PropertyGridModel = new WpfChartPropertyGridModel(this);
		}
		[SkipOnPropertyChangedMethodCall]
		void UpdateRibbonBindings() {
			string[] ribbonBindedProperties = new string[] { "DiagramModel", "LegendModel", "SelectedModel" };
			foreach (string propertyName in ribbonBindedProperties)
				OnPropertyChanged(propertyName);
		}
		void OnSelectedModelChanged() {
			if (selectedModel is WpfChartIndicatorModel)
				SelectedIndicatorAcceptedValueLevels = ((WpfChartIndicatorModel)selectedModel).ValueLevels;
		}
		protected override void UpdateChildren() {
			if (TitleCollectionModel == null)
				TitleCollectionModel = new WpfChartTitleCollectionModel(this, Chart.Titles);
			if (Chart.Diagram == null)
				DiagramModel = null;
			else if (((DiagramModel != null) && (DiagramModel.Diagram != Chart.Diagram)) || ((DiagramModel == null) && (Chart.Diagram != null)))
				DiagramModel = new WpfChartDiagramModel(this, Chart.Diagram);
			if (Chart.Legend == null)
				LegendModel = null;
			else if (((LegendModel != null) && (LegendModel.Legend != Chart.Legend)) || ((LegendModel == null) && (Chart.Legend != null)))
				LegendModel = new WpfChartLegendModel(this, Chart.Legend);
		}
		public IEnumerable<RibbonPageCategoryViewModel> CreateCategoryModels() {
			categories.Add(CreateDefaultCategoryModel());
			categories.Add(CreateSeriesCategoryModel());
			categories.Add(CreateAxisCategoryModel());
			categories.Add(CreateLegendCategoryModel());
			categories.Add(CreateChartTitleCategoryModel());
			categories.Add(CreateConstantLineCategoryViewModel());
			categories.Add(CreateStripOptionsCategory());
			categories.Add(CreateIndicatorOptionsCategory());
			return categories;
		}
		public void DeleteSelectedObject() {
			DeleteCommand.Execute(SelectedObject);
		}
		public void RegisterCommand(ChartCommandBase command) {
			commandsRepository.Add(command);
		}
		public void UpdateCommandsCanExecute() {
			foreach (ChartCommandBase command in commandsRepository)
				command.RaiseCanExecuteChanged();
		}
		public void UpdateModelProperties() {
			UpdateRibbonBindings();
			if (SelectedModel != null)
				SelectedModel.InvalidatePropertyGridModel();
		}
	}
}
