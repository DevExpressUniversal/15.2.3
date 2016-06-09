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
namespace DevExpress.Charts.Designer.Native {
	public abstract class ChartTreeCollectionModel : ChartTreeModel {
		public ChartTreeCollectionModel(string name, string glyph, ChartElementCollectionModelBase dataModel)
			: base(name, glyph, dataModel, false) {
			dataModel.CollectionUpdated += ChartCollectionModelUpdated;
			foreach (ChartModelElement model in dataModel.Children)
				AddChild(CreateSubnode(model));
			IsItemVisibleInTree = (Children.Count > 0);
		}
		void ChartCollectionModelUpdated(ChartCollectionUpdateEventArgs args) {
			UpdateSubnodeCollection(args);
		}
		void UpdateSubnodeCollection(ChartCollectionUpdateEventArgs args) {
			if (args != null) {
				if (args.RemovedItems != null)
					foreach (ChartModelElement model in args.RemovedItems)
						RemoveChild(model);
				if (args.AddedItems != null)
					foreach (InsertedItem item in args.AddedItems)
						InsertChild(item.Index, CreateSubnode(item.Item));
				IsItemVisibleInTree = (Children.Count > 0);
				OnPropertyChanged("Children");
			}
		}
		protected override string GetNameFromDataModel(ChartModelElement dataModel) {
			int count = 0;
			foreach (var child in dataModel.Children)
					count++;
			return BaseName + " (" + count + ")";
		}
		protected abstract ChartTreeModel CreateSubnode(ChartModelElement dataModel);
	}
	public class ChartTitlesCollectionModel : ChartTreeCollectionModel {
		public ChartTitlesCollectionModel(WpfChartTitleCollectionModel dataModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeTitleCollection), GlyphUtils.TreeImages + "TitleCollection", dataModel) { }
		protected override ChartTreeModel CreateSubnode(ChartModelElement dataModel) {
			return new ChartTitleModel((WpfChartTitleModel)dataModel);
		}
	}
	public class ChartTreeAxisCollectionModel : ChartTreeCollectionModel {
		public ChartTreeAxisCollectionModel(string name, string glyph, WpfChartSecondaryAxesCollectionModel dataModel)
			: base(name, glyph, dataModel) { }
		protected override ChartTreeModel CreateSubnode(ChartModelElement dataModel) {
			return new ChartTreeAxisModel(string.Empty, Glyph, (WpfChartAxisModel)dataModel);
		}
	}
	public class ChartTreeSeriesCollectionModel : ChartTreeCollectionModel {
		WpfChartDiagramModel diagramModel;
		public ChartTreeSeriesCollectionModel(WpfChartSeriesCollectionModel dataModel, WpfChartDiagramModel diagramModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeSeriesCollection), GlyphUtils.TreeImages + "SeriesCollection", dataModel) {
			this.diagramModel = diagramModel;
			this.diagramModel.PropertyChanged += DiagramModelPropertyChanged;
			UpdateSeriesTemplateSubnode();
		}
		void DiagramModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "SeriesTemplateModel")
				UpdateSeriesTemplateSubnode();
		}
		void UpdateSeriesTemplateSubnode() {
			SyncModel<WpfChartSeriesModel>(diagramModel.SeriesTemplateModel, typeof(ChartTreeSeriesTemplateModel), (model) => InsertChild(0, new ChartTreeSeriesTemplateModel(diagramModel.SeriesTemplateModel)));
		}
		protected override ChartTreeModel CreateSubnode(ChartModelElement dataModel) {
			return new ChartTreeSeriesModel((WpfChartSeriesModel)dataModel);
		}
	}
	public class ChartTreeConstantLinesCollectionModel : ChartTreeCollectionModel {
		public ChartTreeConstantLinesCollectionModel(WpfChartConstantLinesCollectionModel dataModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeConstantLineCollection), GlyphUtils.TreeImages + "ConstantLineCollection", dataModel) { }
		protected override ChartTreeModel CreateSubnode(ChartModelElement dataModel) {
			return new ChartTreeConstantLineModel(((WpfChartConstantLineModel)dataModel).TitleText, dataModel);
		}
	}
	public class ChartTreePanesCollectionModel : ChartTreeCollectionModel {
		public ChartTreePanesCollectionModel(WpfChartPanesCollectionModel dataModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeAdditionalPanelCollection), GlyphUtils.TreeImages + "PanesSubnode", dataModel) { }
		protected override ChartTreeModel CreateSubnode(ChartModelElement dataModel) {
			return new ChartTreePaneModel((WpfChartPaneModel)dataModel);
		}
	}
	public class ChartTreeIndicatorCollectionModel : ChartTreeCollectionModel {
		public ChartTreeIndicatorCollectionModel(WpfChartIndicatorCollectionModel dataModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeIndicatorCollection), GlyphUtils.TreeImages + "Indicators", dataModel) {			 
		}
		protected override ChartTreeModel CreateSubnode(ChartModelElement dataModel) {
			return new ChartTreeIndicatorModel(((WpfChartIndicatorModel)dataModel).Name, dataModel);
		}
	}
	public class ChartTreeAxisCustomLabelCollectionModel : ChartTreeCollectionModel {
		public ChartTreeAxisCustomLabelCollectionModel(WpfChartAxisCustomLabelCollectionModel dataModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeAxisCustomLabelCollection), GlyphUtils.TreeImages + "AxisCustomLabelCollection", dataModel) { }
		protected override ChartTreeModel CreateSubnode(ChartModelElement dataModel) {
			return new ChartTreeAxisCustomLabelModel(((WpfChartAxisCustomLabelModel)dataModel).DisplayName, dataModel);
		}
	}
	public class ChartTreeStripCollectionModel : ChartTreeCollectionModel {
		public ChartTreeStripCollectionModel(WpfChartStripCollectionModel dataModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeStripCollectionModel), GlyphUtils.TreeImages + "StripCollection", dataModel) { }
		protected override ChartTreeModel CreateSubnode(ChartModelElement dataModel) {
			return new ChartTreeStripModel((WpfChartStripModel)dataModel);
		}
	}
}
