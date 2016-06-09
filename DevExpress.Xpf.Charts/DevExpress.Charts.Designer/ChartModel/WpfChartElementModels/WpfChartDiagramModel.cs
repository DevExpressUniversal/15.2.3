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
using DevExpress.Xpf.Charts;
using System.Collections.ObjectModel;
using DevExpress.Charts.Native;
using System.Windows.Controls;
using System.Collections;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartDiagramModel : ChartModelElement {
		WpfChartSeriesCollectionModel seriesCollectionModel;
		WpfChartAxisModelX primaryAxisModelX;
		WpfChartAxisModelY primaryAxisModelY;
		WpfChartSecondaryAxesCollectionModelX secondaryAxesCollectionModelX;
		WpfChartSecondaryAxesCollectionModelY secondaryAxesCollectionModelY;
		WpfChartPaneModel defaultPaneModel;
		WpfChartPanesCollectionModel panesCollectionModel;
		ObservableCollection<ChartModelElement> allPanes;
		ObservableCollection<ChartModelElement> allAxesX;
		ObservableCollection<ChartModelElement> allAxesY;
		WpfChartSeriesModel seriesTemplateModel;
		DataMemberInfo seriesDataMember;
		public override IEnumerable<ChartModelElement> Children {
			get { return new ChartModelElement[] { PrimaryAxisModelX, PrimaryAxisModelY, SecondaryAxesCollectionModelX, SecondaryAxesCollectionModelY, DefaultPaneModel, PanesCollectionModel, SeriesCollectionModel, SeriesTemplateModel }; }
		}
		public Diagram Diagram {
			get { return (Diagram)ChartElement; }
		}
		public WpfChartSeriesCollectionModel SeriesCollectionModel {
			get { return seriesCollectionModel; }
		}
		public WpfChartSeriesModel SeriesTemplateModel {
			get { 
				return seriesTemplateModel; 
			}
			private set {
				if (seriesTemplateModel != value) {
					seriesTemplateModel = value;
					OnPropertyChanged("SeriesTemplateModel");
				}
			}
		}
		public WpfChartPanesCollectionModel PanesCollectionModel {
			get { return panesCollectionModel; }
		}
		public WpfChartPaneModel DefaultPaneModel {
			get { return defaultPaneModel; }
		}
		public Orientation PaneOrientation {
			get {
				if (Diagram is XYDiagram2D)
					return ((XYDiagram2D)Diagram).PaneOrientation;
				else
					return Orientation.Vertical;
			}
			set {
				if (Diagram is XYDiagram2D) {
					((XYDiagram2D)Diagram).PaneOrientation = value;
					OnPropertyChanged("PaneOrientation");
				}
				else
					ChartDebug.WriteWarning("PaneOrientation property not supported by " + Diagram.GetType().Name + ".");
			}
		}
		public WpfChartAxisModelX PrimaryAxisModelX {
			get { return primaryAxisModelX; }
			set {
				if (primaryAxisModelX != value) {
					primaryAxisModelX = value;
					OnPropertyChanged("PrimaryAxisModelX");
				}
			}
		}
		public WpfChartAxisModelY PrimaryAxisModelY {
			get { return primaryAxisModelY; }
			set {
				if (primaryAxisModelY != value) {
					primaryAxisModelY = value;
					OnPropertyChanged("PrimaryAxisModelY");
				}
			}
		}
		public bool IsRotated {
			get {
				if (Diagram is XYDiagram2D)
					return ((XYDiagram2D)Diagram).Rotated;
				else
					return false;
			}
			set {
				if (Diagram is XYDiagram2D) {
					((XYDiagram2D)Diagram).Rotated = value;
					OnPropertyChanged("IsRotated");
				}
				else
					ChartDebug.WriteWarning("IsRotated property not supported by " + Diagram.GetType().Name + ".");
			}
		}
		public WpfChartSecondaryAxesCollectionModelX SecondaryAxesCollectionModelX {
			get { return secondaryAxesCollectionModelX; }
		}
		public WpfChartSecondaryAxesCollectionModelY SecondaryAxesCollectionModelY {
			get { return secondaryAxesCollectionModelY; }
		}
		public ObservableCollection<ChartModelElement> AllPanes { get { return allPanes; } }
		public ObservableCollection<ChartModelElement> AllAxesX { get { return allAxesX; } }
		public ObservableCollection<ChartModelElement> AllAxesY { get { return allAxesY; } }
		public WpfChartModel ChartModel { get { return Parent as WpfChartModel; } }
		public DataMemberInfo SeriesDataMember {
			get { return seriesDataMember; }
			set {
				if (seriesDataMember.DataMember != value.DataMember) {
					seriesDataMember = value;
					Diagram.SeriesDataMember = seriesDataMember.DataMember;
					OnPropertyChanged("SeriesDataMember");
				}
			}
		}
		public bool EnableAxisXNavigation {
			get {
				if (Diagram is XYDiagram2D)
					return ((XYDiagram2D)Diagram).EnableAxisXNavigation;
				else
					return false;
			}
			set {
				if (Diagram is XYDiagram2D) {
					((XYDiagram2D)Diagram).EnableAxisXNavigation = value;
					OnPropertyChanged("EnableAxisXNavigation");
				}
				else
					ChartDebug.WriteWarning("EnableAxisXNavigation property not supported by " + Diagram.GetType().Name + ".");
			}
		}
		public bool EnableAxisYNavigation {
			get {
				if (Diagram is XYDiagram2D)
					return ((XYDiagram2D)Diagram).EnableAxisYNavigation;
				else
					return false;
			}
			set {
				if (Diagram is XYDiagram2D) {
					((XYDiagram2D)Diagram).EnableAxisYNavigation = value;
					OnPropertyChanged("EnableAxisYNavigation");
				}
				else
					ChartDebug.WriteWarning("EnableAxisYNavigation property not supported by " + Diagram.GetType().Name + ".");
			}
		}
		public int Dimension { 
			get {
				if (Diagram is SimpleDiagram2D)
					return ((SimpleDiagram2D)Diagram).Dimension;
				else if (Diagram is SimpleDiagram3D)
					return ((SimpleDiagram3D)Diagram).Dimension;
				else
					return 3;
			}
			set{
				if (Diagram is SimpleDiagram2D) {
					((SimpleDiagram2D)Diagram).Dimension = value;
					OnPropertyChanged("Dimension");
				}
				else if (Diagram is SimpleDiagram3D) {
					((SimpleDiagram3D)Diagram).Dimension = value;
					OnPropertyChanged("Dimension");
				}
				else
					ChartDebug.WriteWarning("Dimension property not supported by " + Diagram.GetType().Name + ".");
			}
		}
		public LayoutDirection LayoutDirection {
			get {
				if (Diagram is SimpleDiagram2D)
					return ((SimpleDiagram2D)Diagram).LayoutDirection;
				else if (Diagram is SimpleDiagram3D)
					return ((SimpleDiagram3D)Diagram).LayoutDirection;
				else
					return LayoutDirection.Horizontal;
			}
			set {
				if (Diagram is SimpleDiagram2D) {
					((SimpleDiagram2D)Diagram).LayoutDirection = value;
					OnPropertyChanged("LayoutDirection");
				}
				else if (Diagram is SimpleDiagram3D) {
					((SimpleDiagram3D)Diagram).LayoutDirection = value;
					OnPropertyChanged("LayoutDirection");
				}
				else
					ChartDebug.WriteWarning("LayoutDirection property not supported by " + Diagram.GetType().Name + ".");
			}
		}
		public double StartAngle {
			get {
				if (Diagram is CircularDiagram2D)
					return ((CircularDiagram2D)Diagram).StartAngle;
				else
					return 0;
			}
			set {
				if (Diagram is CircularDiagram2D) {
					((CircularDiagram2D)Diagram).StartAngle = value;
					OnPropertyChanged("StartAngle");
				}
				else
					ChartDebug.WriteWarning("StartAngle property not supported by " + Diagram.GetType().Name + ".");
			}
		}
		public CircularDiagramRotationDirection RotationDirection {
			get {
				if (Diagram is CircularDiagram2D)
					return ((CircularDiagram2D)Diagram).RotationDirection;
				else
					return CircularDiagramRotationDirection.Clockwise;
			}
			set {
				if (Diagram is CircularDiagram2D) {
					((CircularDiagram2D)Diagram).RotationDirection = value;
					OnPropertyChanged("RotationDirection");
				}
				else
					ChartDebug.WriteWarning("RotationDirection property not supported by " + Diagram.GetType().Name + ".");
			}
		}
		public CircularDiagramShapeStyle ShapeStyle {
			get {
				if (Diagram is CircularDiagram2D)
					return ((CircularDiagram2D)Diagram).ShapeStyle;
				else
					return CircularDiagramShapeStyle.Circle;
			}
			set {
				if (Diagram is CircularDiagram2D) {
					((CircularDiagram2D)Diagram).ShapeStyle = value;
					OnPropertyChanged("ShapeStyle");
				}
				else
					ChartDebug.WriteWarning("ShapeStyle property not supported by " + Diagram.GetType().Name + ".");
			}
		}
		public double PerspectiveAngle {
			get {
				if (Diagram is Diagram3D)
					return ((Diagram3D)Diagram).PerspectiveAngle;
				else
					return 0;
			}
			set {
				if (Diagram is Diagram3D) {
					((Diagram3D)Diagram).PerspectiveAngle = value;
					OnPropertyChanged("PerspectiveAngle");
				}
				else
					ChartDebug.WriteWarning("PerspectiveAngle property not supported by " + Diagram.GetType().Name + ".");
			}
		}
		public double ZoomPercent {
			get {
				if (Diagram is Diagram3D)
					return ((Diagram3D)Diagram).ZoomPercent;
				else
					return 0;
			}
			set {
				if (Diagram is Diagram3D) {
					((Diagram3D)Diagram).ZoomPercent = value;
					OnPropertyChanged("ZoomPercent");
				}
				else
					ChartDebug.WriteWarning("ZoomPercent property not supported by " + Diagram.GetType().Name + ".");
			}
		}
		public WpfChartDiagramModel(ChartModelElement parent, Diagram diagram) : base(parent, diagram) {
			this.seriesCollectionModel = new WpfChartSeriesCollectionModel(this, Diagram.Series);
			this.primaryAxisModelX = WpfChartAxisModelX.CreateForDiagramPrimaryAxis(this, Diagram);
			this.primaryAxisModelY = WpfChartAxisModelY.CreateForDiagramPrimaryAxis(this, Diagram);
			if (Diagram is XYDiagram2D) {
				var xyDiagram = (XYDiagram2D)Diagram;
				this.secondaryAxesCollectionModelX = new WpfChartSecondaryAxesCollectionModelX(this, xyDiagram.SecondaryAxesX);
				this.secondaryAxesCollectionModelY = new WpfChartSecondaryAxesCollectionModelY(this, xyDiagram.SecondaryAxesY);
				this.panesCollectionModel = new WpfChartPanesCollectionModel(this, xyDiagram.Panes);
				allPanes = new ObservableCollection<ChartModelElement>();
				allAxesX = new ObservableCollection<ChartModelElement>();
				allAxesY = new ObservableCollection<ChartModelElement>();
				UpdatePanes();
				UpdateAxes();
			}
			seriesDataMember = new DataMemberInfo(GetDataMemberName(ChartModel.DataSource, Diagram.SeriesDataMember), Diagram.SeriesDataMember);
			UpdateSeriesTemplate();
			PropertyGridModel = WpfChartDiagramPropertyGridModel.CreatePropertyGridModelForDiagram(this, ChartModel);
		}
		void UpdatePanes() {
			XYDiagram2D xyDiagram = (XYDiagram2D)Diagram;
			if (defaultPaneModel == null || (defaultPaneModel.Pane != xyDiagram.ActualDefaultPane && xyDiagram.ActualDefaultPane != null))
				defaultPaneModel = new WpfChartPaneModel(this, xyDiagram.ActualDefaultPane, ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.DefaultPane));
			allPanes.Clear();
			allPanes.Add(defaultPaneModel);
			PanesCollectionModel.RecursivelyUpdateChildren();
			foreach (ChartModelElement pane in PanesCollectionModel.ModelCollection)
				allPanes.Add(pane);
		}
		void UpdateAxes() {
			allAxesX.Clear();
			allAxesX.Add(primaryAxisModelX);
			SecondaryAxesCollectionModelX.RecursivelyUpdateChildren();
			foreach (ChartModelElement axisX in SecondaryAxesCollectionModelX.ModelCollection)
				allAxesX.Add(axisX);
			allAxesY.Clear();
			allAxesY.Add(primaryAxisModelY);
			SecondaryAxesCollectionModelY.RecursivelyUpdateChildren();
			foreach (ChartModelElement axisY in SecondaryAxesCollectionModelY.ModelCollection)
				allAxesY.Add(axisY);
		}
		void UpdateSeriesTemplate() {
			if (Diagram.SeriesTemplate != null && (SeriesTemplateModel == null || SeriesTemplateModel != null && SeriesTemplateModel.Series != Diagram.SeriesTemplate))
				SeriesTemplateModel = new WpfChartSeriesModel(this, Diagram.SeriesTemplate);
			else
				if (Diagram.SeriesTemplate == null)
					SeriesTemplateModel = null;
		}
		protected override void UpdateChildren() {
			if (Diagram is XYDiagram2D) {
				if (primaryAxisModelX.Axis != ((XYDiagram2D)Diagram).ActualAxisX)
					PrimaryAxisModelX = WpfChartAxisModelX.CreateForDiagramPrimaryAxis(this, Diagram);
				if (primaryAxisModelY.Axis != ((XYDiagram2D)Diagram).ActualAxisY)
					PrimaryAxisModelY = WpfChartAxisModelY.CreateForDiagramPrimaryAxis(this, Diagram);
				UpdatePanes();
				UpdateAxes();
			}
			if (Diagram is PolarDiagram2D){
				if (primaryAxisModelX.Axis != ((PolarDiagram2D)Diagram).ActualAxisX)
					PrimaryAxisModelX = WpfChartAxisModelX.CreateForDiagramPrimaryAxis(this, Diagram);
				if (primaryAxisModelY.Axis != ((PolarDiagram2D)Diagram).ActualAxisY)
					PrimaryAxisModelY = WpfChartAxisModelY.CreateForDiagramPrimaryAxis(this, Diagram);
			}
			if (Diagram is RadarDiagram2D){
				if (primaryAxisModelX.Axis != ((RadarDiagram2D)Diagram).ActualAxisX)
					PrimaryAxisModelX = WpfChartAxisModelX.CreateForDiagramPrimaryAxis(this, Diagram);
				if (primaryAxisModelY.Axis != ((RadarDiagram2D)Diagram).ActualAxisY)
					PrimaryAxisModelY = WpfChartAxisModelY.CreateForDiagramPrimaryAxis(this, Diagram);
			}
			if (Diagram is XYDiagram3D) {
				if (primaryAxisModelX.Axis != ((XYDiagram3D)Diagram).ActualAxisX)
					PrimaryAxisModelX = WpfChartAxisModelX.CreateForDiagramPrimaryAxis(this, Diagram);
				if (primaryAxisModelY.Axis != ((XYDiagram3D)Diagram).ActualAxisY)
					PrimaryAxisModelY = WpfChartAxisModelY.CreateForDiagramPrimaryAxis(this, Diagram);
			}
			UpdateSeriesTemplate();
		}
	}
}
