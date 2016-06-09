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
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Charts;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public abstract class WpfChartDiagramPropertyGridModel : WpfChartElementPropertyGridModelBase {
		public static WpfChartDiagramPropertyGridModel CreatePropertyGridModelForDiagram(WpfChartDiagramModel diagramModel, WpfChartModel chartModel) {
			if (diagramModel.Diagram is XYDiagram2D)
				return new WpfChartXYDiagram2DPropertyGridModel(chartModel, diagramModel);
			if (diagramModel.Diagram is SimpleDiagram2D)
				return new WpfChartSimpleDiagram2DPropertyGridModel(chartModel, diagramModel);
			if (diagramModel.Diagram is XYDiagram3D)
				return new WpfChartXYDiagram3DPropertyGridModel(chartModel, diagramModel);
			if(diagramModel.Diagram is SimpleDiagram3D)
				return new WpfChartSimpleDiagram3DPropertyGridModel(chartModel, diagramModel);
			if (diagramModel.Diagram is CircularDiagram2D)
				return new WpfChartCircularDiagram2DPropertyGridModel(chartModel, diagramModel);
			return null;
		}
		readonly WpfChartDiagramModel diagramModel;
		protected WpfChartDiagramModel DiagramModel { get { return diagramModel; } }
		protected Diagram Diagram { get { return diagramModel.Diagram; } }
		[Category(Categories.Data)]
		public string SeriesDataMember {
			get { return Diagram.SeriesDataMember; }
			set {
				SetProperty("SeriesDataMember", value);
			}
		}
		public WpfChartDiagramPropertyGridModel(WpfChartModel chartModel, WpfChartDiagramModel diagramModel)
			: base(chartModel, "Diagram.") {
			this.diagramModel = diagramModel;
		}
	}
	public class WpfChartXYDiagram2DPropertyGridModel : WpfChartDiagramPropertyGridModel {
		WpfChartXYNavigationOptionsPropertyGridModel navigationOptions;
		PanePropertyGridModelCollection panes;
		SecondaryAxisXPropertyGridModelCollection secondaryAxesX;
		SecondaryAxisYPropertyGridModelCollection secondaryAxesY;
		new XYDiagram2D Diagram { get { return base.Diagram as XYDiagram2D; } }
		[Category(Categories.Behavior)]
		public bool Rotated {
			get { return Diagram.Rotated; }
			set {
				SetProperty("Rotated", value);
			}
		}
		[Category(Categories.Layout)]
		public Orientation PaneOrientation {
			get { return Diagram.PaneOrientation; }
			set {
				SetProperty("PaneOrientation", value);
			}
		}
		[Category(Categories.Navigation)]
		public bool EnableAxisXNavigation {
			get { return Diagram.EnableAxisXNavigation; }
			set {
				SetProperty("EnableAxisXNavigation", value);
			}
		}
		[Category(Categories.Navigation)]
		public bool EnableAxisYNavigation {
			get { return Diagram.EnableAxisYNavigation; }
			set {
				SetProperty("EnableAxisYNavigation", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Navigation)
		]
		public WpfChartXYNavigationOptionsPropertyGridModel NavigationOptions {
			get { return navigationOptions; }
			set { 
				SetProperty("NavigationOptions", new NavigationOptions()); 
			}
		}
		[Category(Categories.Elements)]
		public PanePropertyGridModelCollection Panes {
			get { return panes; }			
		}
		[Category(Categories.Elements)]
		public SecondaryAxisXPropertyGridModelCollection SecondaryAxesX {
			get { return secondaryAxesX; }
		}
		[Category(Categories.Elements)]
		public SecondaryAxisYPropertyGridModelCollection SecondaryAxesY {
			get { return secondaryAxesY; }
		}
		[Category(Categories.Behavior)]
		public double BarDistance {
			get { return Diagram.BarDistance; }
			set {
				SetProperty("BarDistance", value);
			}
		}
		[Category(Categories.Behavior)]
		public int BarDistanceFixed {
			get { return Diagram.BarDistanceFixed; }
			set {
				SetProperty("BarDistanceFixed", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool EqualBarWidth {
			get { return Diagram.EqualBarWidth; }
			set {
				SetProperty("EqualBarWidth", value);
			}
		}
		[Category(Categories.Behavior)]
		public int LabelsResolveOverlappingMinIndent {
			get { return Diagram.LabelsResolveOverlappingMinIndent; }
			set {
				SetProperty("LabelsResolveOverlappingMinIndent", value);
			}
		}
		public WpfChartXYDiagram2DPropertyGridModel(WpfChartModel chartModel, WpfChartDiagramModel diagramModel)
			: base(chartModel, diagramModel) {
			panes = new PanePropertyGridModelCollection(ChartModel);
			if (diagramModel!=null && diagramModel.PanesCollectionModel != null)
				diagramModel.PanesCollectionModel.CollectionUpdated += PanesCollectionUpdated;
			secondaryAxesX = new SecondaryAxisXPropertyGridModelCollection(ChartModel);
			if (diagramModel != null && diagramModel.SecondaryAxesCollectionModelX != null)
				diagramModel.SecondaryAxesCollectionModelX.CollectionUpdated += SecondaryAxesXCollectionUpdated;
			secondaryAxesY = new SecondaryAxisYPropertyGridModelCollection(ChartModel);
			if (diagramModel != null && diagramModel.SecondaryAxesCollectionModelY != null)
				diagramModel.SecondaryAxesCollectionModelY.CollectionUpdated += SecondaryAxesYCollectionUpdated;
			UpdateInternal();
		}
		void UpdateCollection(PropertyGridModelCollectionBase collection, ChartCollectionUpdateEventArgs args) {
			foreach (InsertedItem item in args.AddedItems)
				if (item.Index < collection.Count)
					collection[item.Index].UpdateModelElement(item.Item);
				else
					collection.Insert(item.Index, item.Item.PropertyGridModel);
			List<PropertyGridModelBase> removedModels = new List<PropertyGridModelBase>();
			foreach (ChartModelElement modelElement in args.RemovedItems)
				foreach (PropertyGridModelBase model in collection)
					if (model.ModelElement == modelElement)
						removedModels.Add(model);
			foreach (PropertyGridModelBase removed in removedModels)
				collection.Remove(removed);
		}
		void PanesCollectionUpdated(ChartCollectionUpdateEventArgs args) {
			UpdateCollection(panes, args);
		}
		void SecondaryAxesXCollectionUpdated(ChartCollectionUpdateEventArgs args) {
			UpdateCollection(secondaryAxesX, args);
		}
		void SecondaryAxesYCollectionUpdated(ChartCollectionUpdateEventArgs args) {
			UpdateCollection(secondaryAxesY, args);
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (DiagramModel == null || Diagram == null)
				return;
			if (Diagram.NavigationOptions != null) {
				if (navigationOptions != null && Diagram.NavigationOptions != navigationOptions.NavigationOptions || navigationOptions == null)
					navigationOptions = new WpfChartXYNavigationOptionsPropertyGridModel(ChartModel, Diagram.NavigationOptions);
			}
			else
				navigationOptions = null;
			if (DiagramModel.PanesCollectionModel.ModelCollection.Count > 0) {
				panes.Clear();
				foreach (ChartModelElement paneModel in DiagramModel.PanesCollectionModel.ModelCollection)
					panes.Add(paneModel.PropertyGridModel);
			}
			if (DiagramModel.SecondaryAxesCollectionModelX.ModelCollection.Count > 0) {
				secondaryAxesX.Clear();
				foreach (ChartModelElement axisModel in DiagramModel.SecondaryAxesCollectionModelX.ModelCollection)
					secondaryAxesX.Add(axisModel.PropertyGridModel);
			}
			if (DiagramModel.SecondaryAxesCollectionModelY.ModelCollection.Count > 0) {
				secondaryAxesY.Clear();
				foreach (ChartModelElement axisModel in DiagramModel.SecondaryAxesCollectionModelY.ModelCollection)
					secondaryAxesY.Add(axisModel.PropertyGridModel);
			}
		}
	}
	public class WpfChartSimpleDiagram2DPropertyGridModel : WpfChartDiagramPropertyGridModel {
		new SimpleDiagram2D Diagram { get { return base.Diagram as SimpleDiagram2D; } }
		[Category(Categories.Behavior)]
		public int Dimension {
			get { return Diagram.Dimension; }
			set {
				SetProperty("Dimension", value);
			}
		}
		[Category(Categories.Behavior)]
		public LayoutDirection LayoutDirection {
			get { return Diagram.LayoutDirection; }
			set {
				SetProperty("LayoutDirection", value);
			}
		}
		public WpfChartSimpleDiagram2DPropertyGridModel(WpfChartModel chartModel, WpfChartDiagramModel diagramModel)
			: base(chartModel, diagramModel) {
		}
	}
	public abstract class WpfChartDiagram3DPropertyGridModel : WpfChartDiagramPropertyGridModel {
		WpfChartNavigationOptionsPropertyGridModel navigationOptions;
		new Diagram3D Diagram { get { return base.Diagram as Diagram3D; } }
		[Category(Categories.Presentation)]
		public double PerspectiveAngle {
			get { return Diagram.PerspectiveAngle; }
			set {
				SetProperty("PerspectiveAngle", value);
			}
		}
		[Category(Categories.Presentation)]
		public double ZoomPercent {
			get { return Diagram.ZoomPercent; }
			set {
				SetProperty("ZoomPercent", value);
			}
		}
		[Category(Categories.Presentation)]
		public double VerticalScrollPercent {
			get { return Diagram.VerticalScrollPercent; }
			set {
				SetProperty("VerticalScrollPercent", value);
			}
		}
		[Category(Categories.Presentation)]
		public double HorizontalScrollPercent {
			get { return Diagram.HorizontalScrollPercent; }
			set {
				SetProperty("HorizontalScrollPercent", value);
			}
		}
		[Category(Categories.Navigation)]
		public bool RuntimeRotation {
			get { return Diagram.RuntimeRotation; }
			set {
				SetProperty("RuntimeRotation", value);
			}
		}
		[Category(Categories.Navigation)]
		public bool RuntimeScrolling {
			get { return Diagram.RuntimeScrolling; }
			set {
				SetProperty("RuntimeScrolling", value);
			}
		}
		[Category(Categories.Navigation)]
		public bool RuntimeZooming {
			get { return Diagram.RuntimeZooming; }
			set {
				SetProperty("RuntimeZooming", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Navigation)
		]
		public WpfChartNavigationOptionsPropertyGridModel NavigationOptions {
			get { return navigationOptions; }
			set { SetProperty("NavigationOptions", new NavigationOptions3D()); }
		}
		public WpfChartDiagram3DPropertyGridModel(WpfChartModel chartModel, WpfChartDiagramModel diagramModel)
			: base(chartModel, diagramModel) {
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (DiagramModel == null || Diagram == null)
				return;
			if (Diagram.NavigationOptions != null) {
				if (navigationOptions != null && Diagram.NavigationOptions != navigationOptions.NavigationOptions || navigationOptions == null)
					navigationOptions = new WpfChartNavigationOptionsPropertyGridModel(ChartModel, Diagram.NavigationOptions);
			}
			else
				navigationOptions = null;
		}
	}
	public class WpfChartXYDiagram3DPropertyGridModel : WpfChartDiagram3DPropertyGridModel {
		new XYDiagram3D Diagram { get { return base.Diagram as XYDiagram3D; } }
		[Category(Categories.Brushes)]
		public Brush DomainBrush {
			get { return Diagram.DomainBrush; }
			set {
				SetProperty("DomainBrush", value);
			}
		}
		[Category(Categories.Presentation)]
		public double HeightToWidthRatio {
			get { return Diagram.HeightToWidthRatio; }
			set {
				SetProperty("HeightToWidthRatio", value);
			}
		}
		[Category(Categories.Presentation)]
		public double SeriesDistance {
			get { return Diagram.SeriesDistance; }
			set {
				SetProperty("SeriesDistance", value);
			}
		}
		[Category(Categories.Presentation)]
		public double SeriesPadding {
			get { return Diagram.SeriesPadding; }
			set {
				SetProperty("SeriesPadding", value);
			}
		}
		[Category(Categories.Presentation)]
		public int PlaneDepthFixed {
			get { return Diagram.PlaneDepthFixed; }
			set {
				SetProperty("PlaneDepthFixed", value);
			}
		}
		[Category(Categories.Behavior)]
		public double BarDistance {
			get { return Diagram.BarDistance; }
			set {
				SetProperty("BarDistance", value);
			}
		}
		[Category(Categories.Behavior)]
		public int BarDistanceFixed {
			get { return Diagram.BarDistanceFixed; }
			set {
				SetProperty("BarDistanceFixed", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool EqualBarWidth {
			get { return Diagram.EqualBarWidth; }
			set {
				SetProperty("EqualBarWidth", value);
			}
		}
		public WpfChartXYDiagram3DPropertyGridModel(WpfChartModel chartModel, WpfChartDiagramModel diagramModel)
			: base(chartModel, diagramModel) {
		}
	}
	public class WpfChartSimpleDiagram3DPropertyGridModel : WpfChartDiagram3DPropertyGridModel {
		new SimpleDiagram3D Diagram { get { return base.Diagram as SimpleDiagram3D; } }
		[Category(Categories.Behavior)]
		public int Dimension {
			get { return Diagram.Dimension; }
			set {
				SetProperty("Dimension", value);
			}
		}
		[Category(Categories.Behavior)]
		public LayoutDirection LayoutDirection {
			get { return Diagram.LayoutDirection; }
			set {
				SetProperty("LayoutDirection", value);
			}
		}
		public WpfChartSimpleDiagram3DPropertyGridModel(WpfChartModel chartModel, WpfChartDiagramModel diagramModel)
			: base(chartModel, diagramModel) {
		}
	}
	public class WpfChartCircularDiagram2DPropertyGridModel : WpfChartDiagramPropertyGridModel {
		new CircularDiagram2D Diagram { get { return base.Diagram as CircularDiagram2D; } }
		[Category(Categories.Behavior)]
		public CircularDiagramShapeStyle ShapeStyle {
			get { return Diagram.ShapeStyle; }
			set {
				SetProperty("ShapeStyle", value);
			}
		}
		[Category(Categories.Behavior)]
		public CircularDiagramRotationDirection RotationDirection {
			get { return Diagram.RotationDirection; }
			set {
				SetProperty("RotationDirection", value);
			}
		}
		[Category(Categories.Behavior)]
		public int LabelsResolveOverlappingMinIndent {
			get { return Diagram.LabelsResolveOverlappingMinIndent; }
			set {
				SetProperty("LabelsResolveOverlappingMinIndent", value);
			}
		}
		[Category(Categories.Behavior)]
		public double StartAngle {
			get { return Diagram.StartAngle; }
			set {
				SetProperty("StartAngle", value);
			}
		}
		[Category(Categories.Brushes)]
		public Brush DomainBrush {
			get { return Diagram.DomainBrush; }
			set {
				SetProperty("DomainBrush", value);
			}
		}
		[Category(Categories.Brushes)]
		public Brush DomainBorderBrush {
			get { return Diagram.DomainBorderBrush; }
			set {
				SetProperty("DomainBorderBrush", value);
			}
		}
		public WpfChartCircularDiagram2DPropertyGridModel(WpfChartModel chartModel, WpfChartDiagramModel diagramModel)
			: base(chartModel, diagramModel) {
		}
	}
}
