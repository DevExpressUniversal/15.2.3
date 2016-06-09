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

using DevExpress.Xpf.Grid;
using System.Windows.Media;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public class ChartTreeModelItemSelector : IChildNodesSelector {
		public IEnumerable SelectChildren(object item) {
			ObservableCollection<ChartTreeModel> children = item is ChartTreeModel ? ((ChartTreeModel)item).Children : new ObservableCollection<ChartTreeModel>();
			return children;
		}
	}
	public abstract class ChartTreeModel : ViewModelBase {
		protected delegate ChartTreeModel SyncModelAddChildDelegate<T>(T element);
		ChartTreeModel parent;
		ObservableCollection<ChartTreeModel> children;
		ChartModelElement dataModel;
		string name;
		ImageSource glyphSource;
		string glyph;
		string baseName;
		bool isItemVisibleInTree;
		protected string BaseName {
			get { return baseName; }
		}
		public string Name {
			get { return name; }
			set {
				if (name != value) {
					name = value;
					OnPropertyChanged("Name");
				}
			}
		}
		public string Glyph {
			get { return glyph; }
			set {
				if (glyph != value) {
					glyph = value;
					glyphSource = GlyphUtils.GetGlyphByPath(glyph);
					OnPropertyChanged("Glyph");
					OnPropertyChanged("GlyphSource");
				}
			}
		}
		public ImageSource GlyphSource {
			get { return glyphSource; }
		}
		public ObservableCollection<ChartTreeModel> Children {
			get { return children; }
		}
		public ChartModelElement DataModel {
			get { return dataModel; }
		}
		public bool IsItemVisibleInTree {
			get { return isItemVisibleInTree; }
			set {
				if (isItemVisibleInTree != value) {
					isItemVisibleInTree = value;
					OnPropertyChanged("IsItemVisibleInTree");
				}
			}
		}
		public virtual bool IsVisible {
			get { return true; }
		}
		public ChartTreeModel(string name, string glyph, ChartModelElement dataModel, bool isItemVisibleInTree)
			: base() {
			this.baseName = name;
			Name = name;
			Glyph = glyph;
			this.children = new ObservableCollection<ChartTreeModel>();
			this.dataModel = dataModel;
			if (this.dataModel != null)
				this.dataModel.PropertyChanged += DataModelPropertyChanged;
			Name = GetNameFromDataModel(this.dataModel);
			Glyph = GetGlyphFromDataModel(this.dataModel);
			UpdateChildrenCollection(this.dataModel);
			this.isItemVisibleInTree = isItemVisibleInTree;
		}
		void NotifyRootAboutChanges() {
			if (parent != null)
				parent.NotifyRootAboutChanges();
			else
				OnPropertyChanged("Children");
		}
		protected void SyncModel<T>(T dataModel, Type nodeType, SyncModelAddChildDelegate<T> addChildDelegate) {
			ChartTreeModel child = FindChild(typeof(T), nodeType);
			if (dataModel != null) {
				if ((child != null) && !object.Equals(child.DataModel, dataModel)) {
					RemoveChild(child);
					addChildDelegate(dataModel);
				}
				else if (child == null)
					addChildDelegate(dataModel);
			}
			else {
				if (child != null)
					RemoveChild(child);
			}
		}
		protected void SyncModel<T>(T dataModel, SyncModelAddChildDelegate<T> addChildDelegate) {
			SyncModel<T>(dataModel, null, addChildDelegate);
		}
		protected ChartTreeModel InsertChild(int index, ChartTreeModel item) {
			item.parent = this;
			Children.Insert(index, item);
			return item;
		}
		protected ChartTreeModel AddChild(ChartTreeModel item) {
			item.parent = this;
			Children.Add(item);
			return item;
		}
		protected ChartTreeModel RemoveChild(ChartModelElement model) {
			ChartTreeModel founded = null;
			foreach (var treeItem in children) {
				if (treeItem.DataModel == model)
					founded = treeItem;
			}
			RemoveChild(founded);
			return founded;
		}
		protected ChartTreeModel RemoveChild(ChartTreeModel item) {
			if (item != null)
				children.Remove(item);
			return item;
		}
		protected ChartTreeModel FindChild(Type modelType, Type nodeType) {
			if (modelType != null) {
				foreach (ChartTreeModel child in children) {
					if ((child.DataModel.GetType() == modelType) && ((child.GetType() == nodeType) || (nodeType == null)))
						return child;
				}
			}
			return null;
		}
		protected ChartTreeModel FindRecursively(ChartModelElement model) {
			if (DataModel == model)
				return this;
			foreach (ChartTreeModel node in Children) {
				ChartTreeModel founded = node.FindRecursively(model);
				if (founded != null)
					return founded;
			}
			return null;
		}
		protected virtual void DataModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			UpdateChildrenCollection(dataModel);
			NotifyRootAboutChanges();
			Name = GetNameFromDataModel(dataModel);
			Glyph = GetGlyphFromDataModel(dataModel);
		}
		protected virtual void UpdateChildrenCollection(ChartModelElement dataModel) { }
		protected virtual string GetNameFromDataModel(ChartModelElement dataModel) {
			return BaseName;
		}
		protected virtual string GetGlyphFromDataModel(ChartModelElement dataModel) {
			return this.glyph;
		}
	}
	public class ChartTreeRootModel : ChartTreeModel {
		ChartTreeModel selection;
		WpfChartModel ChartModel {
			get { return (WpfChartModel)DataModel; }
		}
		public ChartTreeModel Selection {
			get {
				return selection;
			}
			set {
				if (selection != value) {
					selection = value;
					if (selection != null) {
						ChartModel.SelectedModel = (selection == null) ? null : selection.DataModel;
						OnPropertyChanged("Selection");
					}
				}
			}
		}
		public ChartTreeRootModel(ChartModelElement dataModel)
			: base("ChartStructure", string.Empty, dataModel, true) {
			selection = FindChild(typeof(WpfChartModel), typeof(ChartTreeChartModel));
		}
		protected override void UpdateChildrenCollection(ChartModelElement dataModel) {
			SyncModel<WpfChartModel>((WpfChartModel)dataModel, (model) => AddChild(new ChartTreeChartModel(model)));
		}
		protected override void DataModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			base.DataModelPropertyChanged(sender, e);
			if (e.PropertyName == "SelectedModel") {
				selection = FindRecursively(ChartModel.SelectedModel);
				OnPropertyChanged("Selection");
			}
		}
	}
	public class ChartTreeChartModel : ChartTreeModel {
		public WpfChartModel ChartModel {
			get {
				return (WpfChartModel)DataModel;
			}
		}
		public ChartTreeChartModel(ChartModelElement dataModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeChart), GlyphUtils.TreeImages + "Chart", dataModel, true) { }
		protected override void UpdateChildrenCollection(ChartModelElement dataModel) {
			WpfChartModel chartModel = (WpfChartModel)dataModel;
			SyncModel<WpfChartTitleCollectionModel>(chartModel.TitleCollectionModel, (model) => AddChild(new ChartTitlesCollectionModel(model)));
			SyncModel<WpfChartLegendModel>(chartModel.LegendModel, (model) => AddChild(new ChartTreeLegendModel(model)));
			SyncModel<WpfChartDiagramModel>(chartModel.DiagramModel, (model) => InsertChild(0, new ChartTreeDiagramModel(chartModel.DiagramModel)));
		}
	}
	public class ChartTreeLegendModel : ChartTreeModel {
		public ChartTreeLegendModel(WpfChartLegendModel dataModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeLegend), GlyphUtils.TreeImages + "Legend", dataModel, true) { }
	}
	public class ChartTitleModel : ChartTreeModel {
		public ChartTitleModel(WpfChartTitleModel dataModel)
			: base(dataModel.DisplayName, GlyphUtils.TreeImages + "Title", dataModel, true) { }
		protected override string GetNameFromDataModel(ChartModelElement dataModel) {
			return ((WpfChartTitleModel)dataModel).DisplayName;
		}
	}
	public class ChartTreeDiagramModel : ChartTreeModel {
		public ChartTreeDiagramModel(WpfChartDiagramModel dataModel)
			: base(ChartDesignerLocalizer.GetLocalizedDiagramTypeName(dataModel.Diagram.GetType()), GlyphUtils.GetTreeDiagramTypeGlyph(dataModel.Diagram.GetType()), dataModel, true) { }
		protected override void UpdateChildrenCollection(ChartModelElement dataModel) {
			var diagramModel = (WpfChartDiagramModel)dataModel;
			SyncModel<WpfChartDiagramModel>(diagramModel, typeof(ChartTreeAxesModel), (model) => AddChild(new ChartTreeAxesModel(diagramModel)));
			SyncModel<WpfChartDiagramModel>(diagramModel, typeof(ChartTreePanesModel), (model) => AddChild(new ChartTreePanesModel(diagramModel)));
			SyncModel<WpfChartSeriesCollectionModel>(diagramModel.SeriesCollectionModel, (model) => InsertChild(0, new ChartTreeSeriesCollectionModel(diagramModel.SeriesCollectionModel, diagramModel)));
		}
	}
	public class ChartTreeAxesModel : ChartTreeModel {
		public ChartTreeAxesModel(WpfChartDiagramModel dataModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeAxesSubnode), GlyphUtils.TreeImages + "AxesSubnode", dataModel, true) {
				IsItemVisibleInTree = (dataModel.Diagram is XYDiagram2D) || (dataModel.Diagram is XYDiagram3D) || (dataModel.Diagram is CircularDiagram2D);
		}
		protected override void UpdateChildrenCollection(ChartModelElement dataModel) {
			var diagramModel = (WpfChartDiagramModel)dataModel;
			SyncModel<WpfChartAxisModelX>(diagramModel.PrimaryAxisModelX, (model) =>
				InsertChild(0, new ChartTreeAxisModel(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreePrimaryAxisX), GlyphUtils.TreeImages + "AxisX", diagramModel.PrimaryAxisModelX)));
			SyncModel<WpfChartAxisModelY>(diagramModel.PrimaryAxisModelY, (model) =>
				InsertChild(1, new ChartTreeAxisModel(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreePrimaryAxisY), GlyphUtils.TreeImages + "AxisY", diagramModel.PrimaryAxisModelY)));
			SyncModel<WpfChartSecondaryAxesCollectionModelX>(diagramModel.SecondaryAxesCollectionModelX, (model) =>
				InsertChild(2, new ChartTreeAxisCollectionModel(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeSecondaryAxesCollectionX), GlyphUtils.TreeImages + "AxisX", diagramModel.SecondaryAxesCollectionModelX)));
			SyncModel<WpfChartSecondaryAxesCollectionModelY>(diagramModel.SecondaryAxesCollectionModelY, (model) =>
				InsertChild(3, new ChartTreeAxisCollectionModel(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeSecondaryAxesCollectionY), GlyphUtils.TreeImages + "AxisY", diagramModel.SecondaryAxesCollectionModelY)));
		}
	}
	public class ChartTreeAxisModel : ChartTreeModel {
		public override bool IsVisible {
			get {
				return ((WpfChartAxisModel)DataModel).IsVisible;
			}
		}
		public ChartTreeAxisModel(string name, string glyph, WpfChartAxisModel dataModel) : base(name, glyph, dataModel, true) { }
		protected override void UpdateChildrenCollection(ChartModelElement dataModel) {
			var axisModel = (WpfChartAxisModel)dataModel;
			SyncModel<WpfChartConstantLinesCollectionModel>(axisModel.ConstantLinesCollectionModel, (model) =>
				AddChild(new ChartTreeConstantLinesCollectionModel(axisModel.ConstantLinesCollectionModel)));
			SyncModel<WpfChartAxisCustomLabelCollectionModel>(axisModel.CustomLabelCollectionModel, (model) =>
				AddChild(new ChartTreeAxisCustomLabelCollectionModel(axisModel.CustomLabelCollectionModel)));
			SyncModel<WpfChartStripCollectionModel>(axisModel.StripCollectionModel, (model) =>
				AddChild(new ChartTreeStripCollectionModel(axisModel.StripCollectionModel)));
		}
		protected override string GetNameFromDataModel(ChartModelElement dataModel) {
			if (string.IsNullOrEmpty(BaseName))
				return ((WpfChartAxisModel)dataModel).Name;
			return BaseName;
		}
		protected override void DataModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			base.DataModelPropertyChanged(sender, e);
			OnPropertyChanged("IsVisible");
		}
	}
	public class ChartTreeSeriesModel : ChartTreeModel {
		public override bool IsVisible {
			get {
				return ((WpfChartSeriesModel)DataModel).IsVisible;
			}
		}
		public ChartTreeSeriesModel(WpfChartSeriesModel dataModel)
			: base(dataModel.Name, GlyphUtils.GetSeriesGlyph(dataModel.Series.GetType()), dataModel, true) {
			IsItemVisibleInTree = !dataModel.IsAutoSeries;
		}
		protected override void UpdateChildrenCollection(ChartModelElement dataModel) {
			var seriesModel = (WpfChartSeriesModel)dataModel;
			bool isItemVisibleInTree = !((WpfChartSeriesModel)DataModel).IsAutoSeries; 
			if (isItemVisibleInTree)
				SyncModel<WpfChartIndicatorCollectionModel>(seriesModel.IndicatorCollectionModel, (model) =>
					AddChild(new ChartTreeIndicatorCollectionModel(seriesModel.IndicatorCollectionModel)));
		}
		protected override string GetNameFromDataModel(ChartModelElement dataModel) {
			WpfChartSeriesModel seriesModel = dataModel as WpfChartSeriesModel;
			if (seriesModel != null)
				return seriesModel.Name;
			return String.Empty;
		}
		protected override void DataModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			base.DataModelPropertyChanged(sender, e);
			OnPropertyChanged("IsVisible");
		}
	}
	public class ChartTreeSeriesTemplateModel : ChartTreeModel {
		public ChartTreeSeriesTemplateModel(WpfChartSeriesModel dataModel)
			: base(dataModel.Name, GlyphUtils.GetSeriesGlyph(dataModel.Series.GetType()), dataModel, true) {
			IsItemVisibleInTree = !dataModel.IsAutoSeries;
		}
		protected override void UpdateChildrenCollection(ChartModelElement dataModel) {
			var seriesModel = (WpfChartSeriesModel)dataModel;
			SyncModel<WpfChartIndicatorCollectionModel>(seriesModel.IndicatorCollectionModel, (model) =>
				AddChild(new ChartTreeIndicatorCollectionModel(seriesModel.IndicatorCollectionModel)));
		}
		protected override string GetNameFromDataModel(ChartModelElement dataModel) {
			WpfChartSeriesModel seriesModel = dataModel as WpfChartSeriesModel;
			if (seriesModel != null)
				return seriesModel.Name + " (" + ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SeriesTemplate) + ")";
			return String.Empty;
		}
	}
	public class ChartTreeConstantLineModel : ChartTreeModel {
		public ChartTreeConstantLineModel(string name, ChartModelElement dataModel)
			: base(name, GlyphUtils.TreeImages + "ConstantLine", dataModel, true) { }
		protected override string GetNameFromDataModel(ChartModelElement dataModel) {
			return ((WpfChartConstantLineModel)dataModel).TitleText;
		}
	}
	public class ChartTreePanesModel : ChartTreeModel {
		public ChartTreePanesModel(WpfChartDiagramModel dataModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreePanesSubnode), GlyphUtils.TreeImages + "PanesSubnode", dataModel, true) {
			IsItemVisibleInTree = dataModel.Diagram is XYDiagram2D;
		}
		protected override void UpdateChildrenCollection(ChartModelElement dataModel) {
			var diagramModel = (WpfChartDiagramModel)dataModel;
			SyncModel<WpfChartPaneModel>(diagramModel.DefaultPaneModel, (model) =>
				AddChild(new ChartTreePaneModel(diagramModel.DefaultPaneModel)));
			SyncModel<WpfChartPanesCollectionModel>(diagramModel.PanesCollectionModel, (model) =>
				AddChild(new ChartTreePanesCollectionModel(diagramModel.PanesCollectionModel)));
		}
	}
	public class ChartTreePaneModel : ChartTreeModel {
		public ChartTreePaneModel(WpfChartPaneModel dataModel)
			: base(string.Empty, GlyphUtils.TreeImages + "Pane", dataModel, true) { }
		protected override string GetNameFromDataModel(ChartModelElement dataModel) {
			return ((WpfChartPaneModel)dataModel).Name;
		}
	}
	public class ChartTreeIndicatorModel : ChartTreeModel {
		public ChartTreeIndicatorModel(string name, ChartModelElement dataModel) : base(name, GlyphUtils.TreeImages + "Indicator", dataModel, true) { 
		}
		protected override string GetNameFromDataModel(ChartModelElement dataModel) {
			var model = (WpfChartIndicatorModel)dataModel;
			return model.Name;
		}
	}
	public class ChartTreeAxisCustomLabelModel : ChartTreeModel {
		public ChartTreeAxisCustomLabelModel(string name, ChartModelElement dataModel) : base(name, GlyphUtils.TreeImages + "CustomLabel", dataModel, true) { }
	}
	public class ChartTreeStripModel : ChartTreeModel {
		public ChartTreeStripModel(WpfChartStripModel dataModel)
			: base(ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.TreeStripModel), GlyphUtils.TreeImages + "Strip", dataModel, true) { }
		protected override string GetNameFromDataModel(ChartModelElement dataModel) {
			return ((WpfChartStripModel)dataModel).LegendText;
		}
	}
}
