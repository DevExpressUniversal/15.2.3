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
using System.Collections.ObjectModel;
namespace DevExpress.Charts.Designer.Native {
	public sealed class DesignerViewModel : ViewModelBase {
		readonly ObservableCollection<RibbonPageCategoryViewModel> categories = new ObservableCollection<RibbonPageCategoryViewModel>();
		readonly WpfChartModel chartModel;
		ChartTreeModel treeModelRoot;
		string title = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartDesignerWindowTitle);
		public string Title {
			get { return title; }
		}
		public ObservableCollection<RibbonPageCategoryViewModel> RibbonPageCategories {
			get { return categories; }
		}
		public ChartTreeModel TreeModelRoot {
			get { return treeModelRoot; }
			private set {
				if (treeModelRoot != value) {
					treeModelRoot = value;
					OnPropertyChanged("TreeModelRoot");
				}
			}
		}
		public string PropertiesDockPanelTitle {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.PropertiesDockPanelTitle); }
		}
		public string SeriesDataDockPanelTitle {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesDataPanelTitle); }
		}
		public string ChartStructureDockPanelTitle {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartStructureDockPanelTitle); }
		}
		public string ClearSeriesDataButtonCaption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ClearSeriesDataButtonCaption); }
		}
		public string EmptyDiagramHint {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.EmptyDiagramHint); }
		}
		public CommandManager CommandManager {
			get { return (ChartModel != null) ? ChartModel.CommandManager : null; }
		}
		public WpfChartModel ChartModel { get { return chartModel; } }
		IEnumerable<RibbonPageCategoryViewModel> models = null;
		public DesignerViewModel(WpfChartModel chartModel) {
			this.chartModel = chartModel;
			if (chartModel != null) {
				RibbonPageCategories.Clear();
				models = chartModel.CreateCategoryModels();
				foreach (var model in models)
					RibbonPageCategories.Add(model);
				TreeModelRoot = new ChartTreeRootModel(chartModel);
			}
		}
		public override void CleanUp() {
			base.CleanUp();
			foreach (var model in models)
				model.CleanUp();
		}
	}
}
