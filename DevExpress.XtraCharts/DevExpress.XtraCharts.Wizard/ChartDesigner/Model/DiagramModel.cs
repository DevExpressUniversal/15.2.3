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

using DevExpress.XtraCharts.Localization;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[HasOptionsControl]
	public abstract class DesignerDiagramModel : DesignerChartElementModelBase {
		readonly Diagram diagram;
		internal Diagram Diagram { get { return diagram; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		protected internal override ChartElement ChartElement { get { return diagram; } }
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.DiagramKey; } }
		public DesignerDiagramModel(Diagram diagram, CommandManager commandManager, Chart chart)
			: base(commandManager, chart) {
			this.diagram = diagram;
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeDiagramElement(this);
		}
	}
	public abstract class Diagram3DModel : DesignerDiagramModel {
		ScrollingOptionsModel scrollingOptionsModel;
		ZoomingOptionsModel zoomingOptionsModel;
		RotationOptionsModel rotationOptionsModel;
		protected new Diagram3D Diagram { get { return (Diagram3D)base.Diagram; } }
		[Category("Behavior")]
		public RotationType RotationType {
			get { return Diagram.RotationType; }
			set { SetProperty("RotationType", value); }
		}
		[Category("Behavior")]
		public RotationOrder RotationOrder {
			get { return Diagram.RotationOrder; }
			set { SetProperty("RotationOrder", value); }
		}
		[Category("Behavior")]
		public int RotationAngleX {
			get { return Diagram.RotationAngleX; }
			set { SetProperty("RotationAngleX", value); }
		}
		[Category("Behavior")]
		public int RotationAngleY {
			get { return Diagram.RotationAngleY; }
			set { SetProperty("RotationAngleY", value); }
		}
		[Category("Behavior")]
		public int RotationAngleZ {
			get { return Diagram.RotationAngleZ; }
			set { SetProperty("RotationAngleZ", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool PerspectiveEnabled {
			get { return Diagram.PerspectiveEnabled; }
			set { SetProperty("PerspectiveEnabled", value); }
		}
		[PropertyForOptions,
		Category("Behavior")]
		public int PerspectiveAngle {
			get { return Diagram.PerspectiveAngle; }
			set { SetProperty("PerspectiveAngle", value); }
		}
		[PropertyForOptions,
		Category("Behavior")]
		public int ZoomPercent {
			get { return Diagram.ZoomPercent; }
			set { SetProperty("ZoomPercent", value); }
		}
		[Category("Behavior")]
		public double HorizontalScrollPercent {
			get { return Diagram.HorizontalScrollPercent; }
			set { SetProperty("HorizontalScrollPercent", value); }
		}
		[Category("Behavior")]
		public double VerticalScrollPercent {
			get { return Diagram.VerticalScrollPercent; }
			set { SetProperty("VerticalScrollPercent", value); }
		}
		[PropertyForOptions,
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool RuntimeRotation {
			get { return Diagram.RuntimeRotation; }
			set { SetProperty("RuntimeRotation", value); }
		}
		[PropertyForOptions,
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool RuntimeZooming {
			get { return Diagram.RuntimeZooming; }
			set { SetProperty("RuntimeZooming", value); }
		}
		[PropertyForOptions,
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool RuntimeScrolling {
			get { return Diagram.RuntimeScrolling; }
			set { SetProperty("RuntimeScrolling", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public ScrollingOptionsModel ScrollingOptions { get { return scrollingOptionsModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public ZoomingOptionsModel ZoomingOptions { get { return zoomingOptionsModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public RotationOptionsModel RotationOptions { get { return rotationOptionsModel; } }
		public Diagram3DModel(Diagram3D diagram, CommandManager commandManager)
			: base(diagram, commandManager, null) {
			this.scrollingOptionsModel = new ScrollingOptionsModel(Diagram.ScrollingOptions, CommandManager);
			this.rotationOptionsModel = new RotationOptionsModel(Diagram.RotationOptions, CommandManager);
			this.zoomingOptionsModel = new ZoomingOptionsModel(Diagram.ZoomingOptions, CommandManager);
			Update();
		}
		protected override void AddChildren() {
			if (scrollingOptionsModel != null)
				Children.Add(scrollingOptionsModel);
			if (rotationOptionsModel != null)
				Children.Add(rotationOptionsModel);
			if (zoomingOptionsModel != null)
				Children.Add(zoomingOptionsModel);
			base.AddChildren();
		}
		public override void Update() {
			if (this.scrollingOptionsModel.ChartElement != Diagram.ScrollingOptions)
				this.scrollingOptionsModel = new ScrollingOptionsModel(Diagram.ScrollingOptions, CommandManager);
			if (this.rotationOptionsModel.ChartElement != Diagram.RotationOptions)
				this.rotationOptionsModel = new RotationOptionsModel(Diagram.RotationOptions, CommandManager);
			if (this.zoomingOptionsModel.ChartElement != Diagram.ZoomingOptions)
				this.zoomingOptionsModel = new ZoomingOptionsModel(Diagram.ZoomingOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(SimpleDiagram3D)), TypeConverter(typeof(SimpleDiagram3DTypeConverter))]
	public class SimpleDiagram3DModel : Diagram3DModel {
		protected new SimpleDiagram3D Diagram { get { return (SimpleDiagram3D)base.Diagram; } }
		[PropertyForOptions,
		Category("Behavior")]
		public LayoutDirection LayoutDirection {
			get { return Diagram.LayoutDirection; }
			set { SetProperty("LayoutDirection", value); }
		}
		[PropertyForOptions,
		Category("Misc")]
		public int Dimension {
			get { return Diagram.Dimension; }
			set { SetProperty("Dimension", value); }
		}
		public SimpleDiagram3DModel(SimpleDiagram3D diagram, CommandManager commandManager)
			: base(diagram, commandManager) {
		}
	}
	[ModelOf(typeof(FunnelDiagram3D)), TypeConverter(typeof(FunnelDiagram3DTypeConverter))]
	public class FunnelDiagram3DModel : SimpleDiagram3DModel {
		public FunnelDiagram3DModel(FunnelDiagram3D diagram, CommandManager commandManager)
			: base(diagram, commandManager) {
		}
	}
	public abstract class XYDiagram2DModel : DesignerDiagramModel {
		RectangleIndentsModel marginsModel;
		XYDiagramDefaultPaneModel defaultPaneModel;
		XYDiagramPaneCollectionModel panesModel;
		ScrollingOptions2DModel scrollingOptionsModel;
		ZoomingOptions2DModel zoomingOptionsModel;
		ChartRangeControlClientDateTimeGridOptionsModel rangeControlDateTimeGridOptionsModel;
		ChartRangeControlClientNumericGridOptionsModel rangeControlNumericGridOptionsModel;
		protected new XYDiagram2D Diagram { get { return (XYDiagram2D)base.Diagram; } }
		[PropertyForOptions("Navigation"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool EnableAxisXScrolling {
			get { return Diagram.EnableAxisXScrolling; }
			set { SetProperty("EnableAxisXScrolling", value); }
		}
		[PropertyForOptions("Navigation"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool EnableAxisYScrolling {
			get { return Diagram.EnableAxisYScrolling; }
			set { SetProperty("EnableAxisYScrolling", value); }
		}
		[PropertyForOptions("Navigation"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool EnableAxisXZooming {
			get { return Diagram.EnableAxisXZooming; }
			set { SetProperty("EnableAxisXZooming", value); }
		}
		[PropertyForOptions("Navigation"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool EnableAxisYZooming {
			get { return Diagram.EnableAxisYZooming; }
			set { SetProperty("EnableAxisYZooming", value); }
		}
		[PropertyForOptions,
		Category("Behavior")]
		public PaneLayoutDirection PaneLayoutDirection {
			get { return Diagram.PaneLayoutDirection; }
			set { SetProperty("PaneLayoutDirection", value); }
		}
		[Category("Behavior")]
		public int PaneDistance {
			get { return Diagram.PaneDistance; }
			set { SetProperty("PaneDistance", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public RectangleIndentsModel Margins { get { return marginsModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public XYDiagramDefaultPaneModel DefaultPane { get { return defaultPaneModel; } }
		[Browsable(false)]
		public XYDiagramPaneCollectionModel Panes { get { return panesModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public ScrollingOptions2DModel ScrollingOptions { get { return scrollingOptionsModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public ZoomingOptions2DModel ZoomingOptions { get { return zoomingOptionsModel; } }
		[Category("Misc")]
		public ChartRangeControlClientDateTimeGridOptionsModel RangeControlDateTimeGridOptions { get { return rangeControlDateTimeGridOptionsModel; } }
		[Category("Misc")]
		public ChartRangeControlClientNumericGridOptionsModel RangeControlNumericGridOptions { get { return rangeControlNumericGridOptionsModel; } }
		public XYDiagram2DModel(XYDiagram2D diagram, CommandManager commandManager, Chart chart)
			: base(diagram, commandManager, chart) {
			this.marginsModel = new RectangleIndentsModel(Diagram.Margins, CommandManager);
			this.defaultPaneModel = new XYDiagramDefaultPaneModel(Diagram.DefaultPane, CommandManager);
			this.panesModel = new XYDiagramPaneCollectionModel(Diagram.Panes, CommandManager, Chart);
			this.scrollingOptionsModel = new ScrollingOptions2DModel(Diagram.ScrollingOptions, CommandManager);
			this.zoomingOptionsModel = new ZoomingOptions2DModel(Diagram.ZoomingOptions, CommandManager);
			this.rangeControlDateTimeGridOptionsModel = new ChartRangeControlClientDateTimeGridOptionsModel(Diagram.RangeControlDateTimeGridOptions, CommandManager);
			this.rangeControlNumericGridOptionsModel = new ChartRangeControlClientNumericGridOptionsModel(Diagram.RangeControlNumericGridOptions, CommandManager);
			Update();
		}
		protected override void AddChildren() {
			if (marginsModel != null)
				Children.Add(marginsModel);
			if (defaultPaneModel != null)
				Children.Add(defaultPaneModel);
			if (panesModel != null)
				Children.Add(panesModel);
			if (scrollingOptionsModel != null)
				Children.Add(scrollingOptionsModel);
			if (zoomingOptionsModel != null)
				Children.Add(zoomingOptionsModel);
			if (rangeControlDateTimeGridOptionsModel != null)
				Children.Add(rangeControlDateTimeGridOptionsModel);
			if (rangeControlNumericGridOptionsModel != null)
				Children.Add(rangeControlNumericGridOptionsModel);
			base.AddChildren();
		}
		public override void Update() {
			if (DesignerChartElementModelBase.NeedRecreate(marginsModel, Diagram.Margins))
				this.marginsModel = new RectangleIndentsModel(Diagram.Margins, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(defaultPaneModel, Diagram.DefaultPane))
				this.defaultPaneModel = new XYDiagramDefaultPaneModel(Diagram.DefaultPane, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(panesModel, Diagram.Panes))
				this.panesModel = new XYDiagramPaneCollectionModel(Diagram.Panes, CommandManager, Chart);
			if (DesignerChartElementModelBase.NeedRecreate(scrollingOptionsModel, Diagram.ScrollingOptions))
				this.scrollingOptionsModel = new ScrollingOptions2DModel(Diagram.ScrollingOptions, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(zoomingOptionsModel, Diagram.ZoomingOptions))
				this.zoomingOptionsModel = new ZoomingOptions2DModel(Diagram.ZoomingOptions, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(rangeControlDateTimeGridOptionsModel, Diagram.RangeControlDateTimeGridOptions))
				this.rangeControlDateTimeGridOptionsModel = new ChartRangeControlClientDateTimeGridOptionsModel(Diagram.RangeControlDateTimeGridOptions, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(rangeControlNumericGridOptionsModel, Diagram.RangeControlNumericGridOptions))
				this.rangeControlNumericGridOptionsModel = new ChartRangeControlClientNumericGridOptionsModel(Diagram.RangeControlNumericGridOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(XYDiagram)), TypeConverter(typeof(XYDiagramTypeConverter))]
	public class XYDiagramModel : XYDiagram2DModel {
		AxisXModel axisX;
		AxisYModel axisY;
		SecondaryAxisXCollectionModel secondaryAxesXModel;
		SecondaryAxisYCollectionModel secondaryAxesYModel;
		protected new XYDiagram Diagram { get { return base.Diagram as XYDiagram; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public AxisXModel AxisX { get { return axisX; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public AxisYModel AxisY { get { return axisY; } }
		[PropertyForOptions,
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool Rotated {
			get { return Diagram.Rotated; }
			set { SetProperty("Rotated", value); }
		}
		[Browsable(false)]
		public SecondaryAxisXCollectionModel SecondaryAxesX { get { return secondaryAxesXModel; } }
		[Browsable(false)]
		public SecondaryAxisYCollectionModel SecondaryAxesY { get { return secondaryAxesYModel; } }
		public XYDiagramModel(XYDiagram diagram, CommandManager commandManager, Chart chart)
			: base(diagram, commandManager, chart) {
			axisX = new AxisXModel(Diagram.AxisX, CommandManager);
			axisY = new AxisYModel(Diagram.AxisY, CommandManager);
			secondaryAxesXModel = new SecondaryAxisXCollectionModel(Diagram.SecondaryAxesX, CommandManager, Chart);
			secondaryAxesYModel = new SecondaryAxisYCollectionModel(Diagram.SecondaryAxesY, CommandManager, Chart);
		}
		protected override void AddChildren() {
			if (axisX != null)
				Children.Add(axisX);
			if (axisY != null)
				Children.Add(axisY);
			if (secondaryAxesXModel != null)
				Children.Add(secondaryAxesXModel);
			if (secondaryAxesYModel != null)
				Children.Add(secondaryAxesYModel);
			base.AddChildren();
		}
		public override void Update() {
			if (Diagram != null) {
				if (DesignerChartElementModelBase.NeedRecreate(axisX, Diagram.AxisX))
					axisX = new AxisXModel(Diagram.AxisX, CommandManager);
				if (DesignerChartElementModelBase.NeedRecreate(axisY, Diagram.AxisY))
					axisY = new AxisYModel(Diagram.AxisY, CommandManager);
				if (DesignerChartElementModelBase.NeedRecreate(secondaryAxesXModel, Diagram.SecondaryAxesX))
					secondaryAxesXModel = new SecondaryAxisXCollectionModel(Diagram.SecondaryAxesX, CommandManager, Chart);
				if (DesignerChartElementModelBase.NeedRecreate(secondaryAxesYModel, Diagram.SecondaryAxesY))
					secondaryAxesYModel = new SecondaryAxisYCollectionModel(Diagram.SecondaryAxesY, CommandManager, Chart);
				ClearChildren();
				AddChildren();
			}
			else {
				axisX = null;
				axisY = null;
				secondaryAxesXModel = null;
				secondaryAxesYModel = null;
			}
			base.Update();
		}
	}
	[ModelOf(typeof(SwiftPlotDiagram)), TypeConverter(typeof(SwiftPlotDiagramTypeConverter))]
	public class SwiftPlotDiagramModel : XYDiagram2DModel {
		SwiftPlotDiagramAxisXModel axisXModel;
		SwiftPlotDiagramAxisYModel axisYModel;
		SwiftPlotDiagramSecondaryAxisXCollectionModel secondaryAxesXModel;
		SwiftPlotDiagramSecondaryAxisYCollectionModel secondaryAxesYModel;
		protected new SwiftPlotDiagram Diagram { get { return (SwiftPlotDiagram)base.Diagram; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public SwiftPlotDiagramAxisXModel AxisX { get { return axisXModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public SwiftPlotDiagramAxisYModel AxisY { get { return axisYModel; } }
		[Browsable(false)]
		public SwiftPlotDiagramSecondaryAxisXCollectionModel SecondaryAxesX { get { return secondaryAxesXModel; } }
		[Browsable(false)]
		public SwiftPlotDiagramSecondaryAxisYCollectionModel SecondaryAxesY { get { return secondaryAxesYModel; } }
		public SwiftPlotDiagramModel(SwiftPlotDiagram diagram, CommandManager commandManager, Chart chart)
			: base(diagram, commandManager, chart) {
			this.axisXModel = new SwiftPlotDiagramAxisXModel(Diagram.AxisX, CommandManager);
			this.axisYModel = new SwiftPlotDiagramAxisYModel(Diagram.AxisY, CommandManager);
			this.secondaryAxesXModel = new SwiftPlotDiagramSecondaryAxisXCollectionModel(Diagram.SecondaryAxesX, CommandManager, Chart);
			this.secondaryAxesYModel = new SwiftPlotDiagramSecondaryAxisYCollectionModel(Diagram.SecondaryAxesY, CommandManager, Chart);
			Update();
		}
		protected override void AddChildren() {
			if (axisXModel != null)
				Children.Add(axisXModel);
			if (axisYModel != null)
				Children.Add(axisYModel);
			if (secondaryAxesXModel != null)
				Children.Add(secondaryAxesXModel);
			if (secondaryAxesYModel != null)
				Children.Add(secondaryAxesYModel);
			base.AddChildren();
		}
		public override void Update() {
			if (DesignerChartElementModelBase.NeedRecreate(axisXModel, Diagram.AxisX))
				this.axisXModel = new SwiftPlotDiagramAxisXModel(Diagram.AxisX, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(axisYModel, Diagram.AxisY))
				this.axisYModel = new SwiftPlotDiagramAxisYModel(Diagram.AxisY, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(secondaryAxesXModel, Diagram.SecondaryAxesX))
				this.secondaryAxesXModel = new SwiftPlotDiagramSecondaryAxisXCollectionModel(Diagram.SecondaryAxesX, CommandManager, Chart);
			if (DesignerChartElementModelBase.NeedRecreate(secondaryAxesYModel, Diagram.SecondaryAxesY))
				this.secondaryAxesYModel = new SwiftPlotDiagramSecondaryAxisYCollectionModel(Diagram.SecondaryAxesY, CommandManager, Chart);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(GanttDiagram)), TypeConverter(typeof(GanttDiagramTypeConverter))]
	public class GanttDiagramModel : XYDiagramModel {
		protected new GanttDiagram Diagram { get { return (GanttDiagram)base.Diagram; } }
		[Browsable(false)]
		public new bool Rotated { get; set; }
		public GanttDiagramModel(GanttDiagram diagram, CommandManager commandManager, Chart chart)
			: base(diagram, commandManager, chart) {
		}
	}
	[ModelOf(typeof(RadarDiagram))]
	public class RadarDiagramModel : DesignerDiagramModel {
		PolygonFillStyleModel fillStyleModel;
		ShadowModel shadowModel;
		RadarAxisXModel axisXModel;
		RadarAxisYModel axisYModel;
		BackgroundImageModel backImageModel;
		RectangleIndentsModel marginsModel;
		protected new RadarDiagram Diagram { get { return (RadarDiagram)base.Diagram; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public PolygonFillStyleModel FillStyle { get { return fillStyleModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public ShadowModel Shadow { get { return shadowModel; } }
		[Category("Appearance")]
		public Color BackColor {
			get { return Diagram.BackColor; }
			set { SetProperty("BackColor", value); }
		}
		[Category("Appearance"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool BorderVisible {
			get { return Diagram.BorderVisible; }
			set { SetProperty("BorderVisible", value); }
		}
		[Category("Appearance")]
		public Color BorderColor {
			get { return Diagram.BorderColor; }
			set { SetProperty("BorderColor", value); }
		}
		[PropertyForOptions,
		Category("Appearance")]
		public double StartAngleInDegrees {
			get { return Diagram.StartAngleInDegrees; }
			set { SetProperty("StartAngleInDegrees", value); }
		}
		[PropertyForOptions,
		Category("Appearance")]
		public RadarDiagramRotationDirection RotationDirection {
			get { return Diagram.RotationDirection; }
			set { SetProperty("RotationDirection", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public RadarAxisXModel AxisX { get { return axisXModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public RadarAxisYModel AxisY { get { return axisYModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public BackgroundImageModel BackImage { get { return backImageModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public RectangleIndentsModel Margins { get { return marginsModel; } }
		[PropertyForOptions,
		Category("Appearance")]
		public RadarDiagramDrawingStyle DrawingStyle {
			get { return Diagram.DrawingStyle; }
			set { SetProperty("DrawingStyle", value); }
		}
		public RadarDiagramModel(RadarDiagram diagram, CommandManager commandManager)
			: base(diagram, commandManager, null) {
			this.fillStyleModel = new PolygonFillStyleModel(Diagram.FillStyle, CommandManager);
			this.shadowModel = new ShadowModel(Diagram.Shadow, CommandManager);
			this.axisXModel = CreateRadarAxisXModel();
			this.axisYModel = new RadarAxisYModel(Diagram.AxisY, CommandManager);
			this.backImageModel = new BackgroundImageModel(Diagram.BackImage, CommandManager);
			this.marginsModel = new RectangleIndentsModel(Diagram.Margins, CommandManager);
			Update();
		}
		protected override void AddChildren() {
			if (fillStyleModel != null)
				Children.Add(fillStyleModel);
			if (shadowModel != null)
				Children.Add(shadowModel);
			if (axisXModel != null)
				Children.Add(axisXModel);
			if (axisYModel != null)
				Children.Add(axisYModel);
			if (backImageModel != null)
				Children.Add(backImageModel);
			if (marginsModel != null)
				Children.Add(marginsModel);
			base.AddChildren();
		}
		protected virtual RadarAxisXModel CreateRadarAxisXModel() {
			return new RadarAxisXModel(Diagram.AxisX, CommandManager);
		}
		public override void Update() {
			if (DesignerChartElementModelBase.NeedRecreate(fillStyleModel, Diagram.FillStyle))
				this.fillStyleModel = new PolygonFillStyleModel(Diagram.FillStyle, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(shadowModel, Diagram.Shadow))
				this.shadowModel = new ShadowModel(Diagram.Shadow, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(axisXModel, Diagram.AxisX))
				this.axisXModel = CreateRadarAxisXModel();
			if (DesignerChartElementModelBase.NeedRecreate(axisYModel, Diagram.AxisY))
				this.axisYModel = new RadarAxisYModel(Diagram.AxisY, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(backImageModel, Diagram.BackImage))
				this.backImageModel = new BackgroundImageModel(Diagram.BackImage, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(marginsModel, Diagram.Margins))
				this.marginsModel = new RectangleIndentsModel(Diagram.Margins, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(PolarDiagram))]
	public class PolarDiagramModel : RadarDiagramModel {
		protected new PolarDiagram Diagram { get { return (PolarDiagram)base.Diagram; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public new PolarAxisXModel AxisX { get { return (PolarAxisXModel)base.AxisX; } }
		protected override RadarAxisXModel CreateRadarAxisXModel() {
			return new PolarAxisXModel((PolarAxisX)Diagram.AxisX, CommandManager);
		}
		public PolarDiagramModel(PolarDiagram diagram, CommandManager commandManager)
			: base(diagram, commandManager) {
		}
	}
	[ModelOf(typeof(SimpleDiagram)), TypeConverter(typeof(SimpleDiagramTypeConverter))]
	public class SimpleDiagramModel : DesignerDiagramModel {
		RectangleIndentsModel marginsModel;
		protected new SimpleDiagram Diagram { get { return (SimpleDiagram)base.Diagram; } }
		[Category("Layout"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool EqualPieSize {
			get { return Diagram.EqualPieSize; }
			set { SetProperty("EqualPieSize", value); }
		}
		[PropertyForOptions,
		Category("Behavior")]
		public LayoutDirection LayoutDirection {
			get { return Diagram.LayoutDirection; }
			set { SetProperty("LayoutDirection", value); }
		}
		[PropertyForOptions,
		Category("Misc")]
		public int Dimension {
			get { return Diagram.Dimension; }
			set { SetProperty("Dimension", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public RectangleIndentsModel Margins { get { return marginsModel; } }
		public SimpleDiagramModel(SimpleDiagram diagram, CommandManager commandManager)
			: base(diagram, commandManager, null) {
			Update();
		}
		protected override void AddChildren() {
			if (marginsModel != null)
				Children.Add(marginsModel);
			base.AddChildren();
		}
		public override void Update() {
			this.marginsModel = new RectangleIndentsModel(Diagram.Margins, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(XYDiagram3D)), TypeConverter(typeof(XYDiagram3DTypeConverter))]
	public class XYDiagram3DModel : Diagram3DModel {
		RectangleFillStyle3DModel fillStyleModel;
		AxisX3DModel axisXModel;
		AxisY3DModel axisYModel;
		BackgroundImageModel backImageModel;
		protected new XYDiagram3D Diagram { get { return (XYDiagram3D)base.Diagram; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public RectangleFillStyle3DModel FillStyle { get { return fillStyleModel; } }
		[Category("Appearance")]
		public Color BackColor {
			get { return Diagram.BackColor; }
			set { SetProperty("BackColor", value); }
		}
		[Category("Misc")]
		public double SeriesDistance {
			get { return Diagram.SeriesDistance; }
			set { SetProperty("SeriesDistance", value); }
		}
		[Category("Misc")]
		public int SeriesDistanceFixed {
			get { return Diagram.SeriesDistanceFixed; }
			set { SetProperty("SeriesDistanceFixed", value); }
		}
		[Category("Misc")]
		public int SeriesIndentFixed {
			get { return Diagram.SeriesIndentFixed; }
			set { SetProperty("SeriesIndentFixed", value); }
		}
		[Category("Misc")]
		public int PlaneDepthFixed {
			get { return Diagram.PlaneDepthFixed; }
			set { SetProperty("PlaneDepthFixed", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public AxisX3DModel AxisX { get { return axisXModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Elements")]
		public AxisY3DModel AxisY { get { return axisYModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public BackgroundImageModel BackImage { get { return backImageModel; } }
		public XYDiagram3DModel(XYDiagram3D diagram, CommandManager commandManager)
			: base(diagram, commandManager) {
			this.fillStyleModel = new RectangleFillStyle3DModel(Diagram.FillStyle, CommandManager);
			this.axisXModel = new AxisX3DModel(Diagram.AxisX, CommandManager);
			this.axisYModel = new AxisY3DModel(Diagram.AxisY, CommandManager);
			this.backImageModel = new BackgroundImageModel(Diagram.BackImage, CommandManager);
			Update();
		}
		protected override void AddChildren() {
			if (fillStyleModel != null)
				Children.Add(fillStyleModel);
			if (axisXModel != null)
				Children.Add(axisXModel);
			if (axisYModel != null)
				Children.Add(axisYModel);
			if (backImageModel != null)
				Children.Add(backImageModel);
			base.AddChildren();
		}
		public override void Update() {
			if (DesignerChartElementModelBase.NeedRecreate(fillStyleModel, Diagram.FillStyle))
				this.fillStyleModel = new RectangleFillStyle3DModel(Diagram.FillStyle, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(axisXModel, Diagram.AxisX))
				this.axisXModel = new AxisX3DModel(Diagram.AxisX, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(axisYModel, Diagram.AxisY))
				this.axisYModel = new AxisY3DModel(Diagram.AxisY, CommandManager);
			if (DesignerChartElementModelBase.NeedRecreate(backImageModel, Diagram.BackImage))
				this.backImageModel = new BackgroundImageModel(Diagram.BackImage, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
}
