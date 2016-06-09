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

using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraEditors;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[HasOptionsControl]
	public abstract class AnnotationModel : ChartElementNamedModel, ISupportModelVisibility {
		CustomBorderModel borderModel;
		RectangleFillStyleModel fillStyleModel;
		RectangleIndentsModel paddingModel;
		ShadowModel shadowModel;
		AnnotationAnchorPointModel anchorPointModel;
		AnnotationShapePositionModel shapePositionModel;
		protected internal Annotation Annotation { get { return (Annotation)base.ChartElementNamed; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		[Category("Layout")]
		public int Angle {
			get { return Annotation.Angle; }
			set { SetProperty("Angle", value); }
		}
		[
		PropertyForOptions(-1, "Appearance"),
		Category("Appearance"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool Visible {
			get { return Annotation.Visible; }
			set { SetProperty("Visible", value); }
		}
		[PropertyForOptions("Appearance"),
		Category("Appearance")]
		public Color BackColor {
			get { return Annotation.BackColor; }
			set { SetProperty("BackColor", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public CustomBorderModel Border { get { return borderModel; } }
		[PropertyForOptions("Appearance"),
		Category("Appearance")]
		public AnnotationConnectorStyle ConnectorStyle {
			get { return Annotation.ConnectorStyle; }
			set { SetProperty("ConnectorStyle", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public RectangleFillStyleModel FillStyle { get { return fillStyleModel; } }
		[
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool LabelMode {
			get { return Annotation.LabelMode; }
			set { SetProperty("LabelMode", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public RectangleIndentsModel Padding { get { return paddingModel; } }
		[PropertyForOptions("Appearance"),
		Category("Appearance")]
		public ShapeKind ShapeKind {
			get { return Annotation.ShapeKind; }
			set { SetProperty("ShapeKind", value); }
		}
		[
		PropertyForOptions("Appearance"),
		DependentUpon("ShapeKind"),
		Category("Appearance")]
		public int ShapeFillet {
			get { return Annotation.ShapeFillet; }
			set { SetProperty("ShapeFillet", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Appearance")]
		public ShadowModel Shadow { get { return shadowModel; } }
		[Category("Layout")]
		public int Height {
			get { return Annotation.Height; }
			set { SetProperty("Height", value); }
		}
		[Category("Layout")]
		public int Width {
			get { return Annotation.Width; }
			set { SetProperty("Width", value); }
		}
		[
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool RuntimeAnchoring {
			get { return Annotation.RuntimeAnchoring; }
			set { SetProperty("RuntimeAnchoring", value); }
		}
		[
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool RuntimeMoving {
			get { return Annotation.RuntimeMoving; }
			set { SetProperty("RuntimeMoving", value); }
		}
		[
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool RuntimeResizing {
			get { return Annotation.RuntimeResizing; }
			set { SetProperty("RuntimeResizing", value); }
		}
		[
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool RuntimeRotation {
			get { return Annotation.RuntimeRotation; }
			set { SetProperty("RuntimeRotation", value); }
		}
		[Category("Behavior")]
		public int ZOrder {
			get { return Annotation.ZOrder; }
			set { SetProperty("ZOrder", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor(typeof(AnnotationAnchorPointModelPickerEditor), typeof(UITypeEditor)),
		PropertyForOptions,
		AllocateToGroup("Layout"),
		Category("Layout")
		]
		public AnnotationAnchorPointModel AnchorPoint {
			get { return anchorPointModel; }
			set { SetProperty("AnchorPoint", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor(typeof(AnnotationShapePositionModelPickerEditor), typeof(UITypeEditor)),
		PropertyForOptions,
		AllocateToGroup("Layout"),
		Category("Layout")
		]
		public AnnotationShapePositionModel ShapePosition {
			get { return shapePositionModel; }
			set { SetProperty("ShapePosition", value); }
		}
		public AnnotationModel(Annotation annotation, CommandManager commandManager)
			: base(annotation, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (borderModel != null)
				Children.Add(borderModel);
			if (fillStyleModel != null)
				Children.Add(fillStyleModel);
			if (paddingModel != null)
				Children.Add(paddingModel);
			if (shadowModel != null)
				Children.Add(shadowModel);
			if (anchorPointModel != null)
				Children.Add(anchorPointModel);
			if (shapePositionModel != null)
				Children.Add(shapePositionModel);
			base.AddChildren();
		}
		public override void Update() {
			this.borderModel = new CustomBorderModel(Annotation.Border, CommandManager);
			this.fillStyleModel = new RectangleFillStyleModel(Annotation.FillStyle, CommandManager);
			this.paddingModel = new RectangleIndentsModel(Annotation.Padding, CommandManager);
			this.shadowModel = new ShadowModel(Annotation.Shadow, CommandManager);
			this.anchorPointModel = ModelHelper.CreateAnnotationAnchorPointModelInstance(Annotation.AnchorPoint, CommandManager);
			this.shapePositionModel = ModelHelper.CreateAnnotationShapePositionModelInstance(Annotation.ShapePosition, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
	}
	[
	ModelOf(typeof(ImageAnnotation)),
	TypeConverter(typeof(ImageAnnotationTypeConverter))
	]
	public class ImageAnnotationModel : AnnotationModel {
		ChartImageModel imageModel;
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.ImageAnnotationKey; } }
		protected new ImageAnnotation Annotation { get { return (ImageAnnotation)base.Annotation; } }
		[
		Category("Behavior"),
		TypeConverter(typeof(EnumTypeConverter))
		]
		public ChartImageSizeMode SizeMode {
			get { return Annotation.SizeMode; }
			set { SetProperty("SizeMode", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Behavior")]
		public ChartImageModel Image { get { return imageModel; } }
		public ImageAnnotationModel(ImageAnnotation annotation, CommandManager commandManager)
			: base(annotation, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (imageModel != null)
				Children.Add(imageModel);
			base.AddChildren();
		}
		public override void Update() {
			this.imageModel = new ChartImageModel(Annotation.Image, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[
	ModelOf(typeof(TextAnnotation)),
	TypeConverter(typeof(TextAnnotationTypeConverter))
	]
	public class TextAnnotationModel : AnnotationModel {
		protected internal override string ChartTreeImageKey { get { return DesignerImageKeys.TextAnnotationKey; } }
		protected new TextAnnotation Annotation { get { return (TextAnnotation)base.Annotation; } }
		[
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool AutoSize {
			get { return Annotation.AutoSize; }
			set { SetProperty("AutoSize", value); }
		}
		[
		Category("Appearance"),
		TypeConverter(typeof(DefaultBooleanConverter))
		]
		public DefaultBoolean EnableAntialiasing {
			get { return Annotation.EnableAntialiasing; }
			set { SetProperty("EnableAntialiasing", value); }
		}
		[
		PropertyForOptions("Behavior"),
		Category("Behavior"),
		TypeConverter(typeof(StringAlignmentTypeConvertor))
		]
		public StringAlignment TextAlignment {
			get { return Annotation.TextAlignment; }
			set { SetProperty("TextAlignment", value); }
		}
		[
		Category("Appearance"),
		TypeConverter(typeof(FontTypeConverter))
		]
		public Font Font {
			get { return Annotation.Font; }
			set { SetProperty("Font", value); }
		}
		[PropertyForOptions("Behavior"),
		UseEditor(typeof(MemoEdit), typeof(StrintgToLinesAdapter)),
		Category("Behavior")]
		public string Text {
			get { return Annotation.Text; }
			set { SetProperty("Text", value); }
		}
		[PropertyForOptions("Appearance"),
		Category("Appearance")]
		public Color TextColor {
			get { return Annotation.TextColor; }
			set { SetProperty("TextColor", value); }
		}
		[Editor(typeof(StringCollectionEditor), typeof(UITypeEditor)),
		Category("Behavior")]
		public string[] Lines {
			get { return Annotation.Lines; }
			set { SetProperty("Lines", value); }
		}
		public TextAnnotationModel(TextAnnotation annotation, CommandManager commandManager)
			: base(annotation, commandManager) {
		}
	}
	[GenerateHeritableProperties]
	public abstract class AnnotationAnchorPointModel : DesignerChartElementModelBase {
		readonly AnnotationAnchorPoint anchorPoint;
		protected AnnotationAnchorPoint AnchorPoint { get { return anchorPoint; } }
		protected internal override ChartElement ChartElement { get { return anchorPoint; } }
		public AnnotationAnchorPointModel(AnnotationAnchorPoint anchorPoint, CommandManager commandManager)
			: base(commandManager) {
			this.anchorPoint = anchorPoint;
		}
	}
	[ModelOf(typeof(ChartAnchorPoint))]
	public class ChartAnchorPointModel : AnnotationAnchorPointModel {
		protected new ChartAnchorPoint AnchorPoint { get { return (ChartAnchorPoint)base.AnchorPoint; } }
		[PropertyForOptions,
		Category("General")]
		public int X {
			get { return AnchorPoint.X; }
			set { SetProperty("X", value); }
		}
		[PropertyForOptions,
		Category("General")]
		public int Y {
			get { return AnchorPoint.Y; }
			set { SetProperty("Y", value); }
		}
		public ChartAnchorPointModel(ChartAnchorPoint anchorPoint, CommandManager commandManager)
			: base(anchorPoint, commandManager) {
		}
	}
	[ModelOf(typeof(PaneAnchorPoint))]
	public class PaneAnchorPointModel : AnnotationAnchorPointModel {
		XYDiagramPaneBaseModel paneModel;
		AxisXCoordinateModel axisXCoordinateModel;
		AxisYCoordinateModel axisYCoordinateModel;
		protected new PaneAnchorPoint AnchorPoint { get { return (PaneAnchorPoint)base.AnchorPoint; } }
		[
		Editor("DevExpress.XtraCharts.Designer.Native.PaneModelUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		UseAsSimpleProperty,
		PropertyForOptions,
		Category("General")]
		public XYDiagramPaneBaseModel Pane {
			get { return paneModel; }
			set { SetProperty("Pane", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		Category("General")]
		public AxisXCoordinateModel AxisXCoordinate { get { return axisXCoordinateModel; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		Category("General")]
		public AxisYCoordinateModel AxisYCoordinate { get { return axisYCoordinateModel; } }
		public PaneAnchorPointModel(PaneAnchorPoint anchorPoint, CommandManager commandManager)
			: base(anchorPoint, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (paneModel != null)
				Children.Add(paneModel);
			if (axisXCoordinateModel != null)
				Children.Add(axisXCoordinateModel);
			if (axisYCoordinateModel != null)
				Children.Add(axisYCoordinateModel);
			base.AddChildren();
		}
		public override void Update() {
			this.paneModel = ModelHelper.CreatePaneModelInstance(AnchorPoint.Pane, CommandManager);
			this.axisXCoordinateModel = new AxisXCoordinateModel(AnchorPoint.AxisXCoordinate, CommandManager);
			this.axisYCoordinateModel = new AxisYCoordinateModel(AnchorPoint.AxisYCoordinate, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(SeriesPointAnchorPoint))]
	public class SeriesPointAnchorPointModel : AnnotationAnchorPointModel {
		SeriesPointModel seriesPointModel;
		protected new SeriesPointAnchorPoint AnchorPoint { get { return (SeriesPointAnchorPoint)base.AnchorPoint; } }
		[Editor(typeof(AnchorPointSeriesPointModelEditor), typeof(UITypeEditor)),
		Category("Misc")]
		public SeriesPointModel SeriesPoint {
			get { return seriesPointModel; }
			set { SetProperty("SeriesPoint", value); }
		}
		public SeriesPointAnchorPointModel(SeriesPointAnchorPoint anchorPoint, CommandManager commandManager)
			: base(anchorPoint, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (seriesPointModel != null)
				Children.Add(seriesPointModel);
			base.AddChildren();
		}
		public override void Update() {
			this.seriesPointModel = ((IOwnedElement)AnchorPoint).ChartContainer != null && AnchorPoint.SeriesPoint != null ? new SeriesPointModel(AnchorPoint.SeriesPoint, CommandManager) : null;
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[GenerateHeritableProperties]
	public abstract class AnnotationShapePositionModel : DesignerChartElementModelBase {
		readonly AnnotationShapePosition shapePosition;
		protected AnnotationShapePosition ShapePosition { get { return shapePosition; } }
		protected internal override ChartElement ChartElement { get { return shapePosition; } }
		public AnnotationShapePositionModel(AnnotationShapePosition shapePosition, CommandManager commandManager)
			: base(commandManager) {
			this.shapePosition = shapePosition;
		}
	}
	[ModelOf(typeof(FreePosition))]
	public class FreePositionModel : AnnotationShapePositionModel {
		RectangleIndentsModel innerIndentsModel;
		RectangleIndentsModel outerIndentsModel;
		DesignerChartElementModelBase dockTargetModel;
		protected new FreePosition ShapePosition { get { return (FreePosition)base.ShapePosition; } }
		[PropertyForOptions,
		Category("General")]
		public DockCorner DockCorner {
			get { return ShapePosition.DockCorner; }
			set { SetProperty("DockCorner", value); }
		}
		[Editor(typeof(DockTargetModelUITypeEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(DockTargetModelTypeConverter))]
		public DesignerChartElementModelBase DockTarget {
			get { return dockTargetModel; }
			set { SetProperty("DockTarget", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		Category("General")]
		public RectangleIndentsModel InnerIndents { get { return innerIndentsModel; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		Category("General")]
		public RectangleIndentsModel OuterIndents { get { return outerIndentsModel; } }
		public FreePositionModel(FreePosition shapePosition, CommandManager commandManager)
			: base(shapePosition, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (innerIndentsModel != null)
				Children.Add(innerIndentsModel);
			if (outerIndentsModel != null)
				Children.Add(outerIndentsModel);
			base.AddChildren();
		}
		public override void Update() {
			DesignerChartModel chartModel = FindParent<DesignerChartModel>();
			if (chartModel == null)
				return;
			this.innerIndentsModel = new RectangleIndentsModel(ShapePosition.InnerIndents, CommandManager);
			this.outerIndentsModel = new RectangleIndentsModel(ShapePosition.OuterIndents, CommandManager);
			this.dockTargetModel = chartModel.FindElementModel(ShapePosition.DockTarget);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(RelativePosition))]
	public class RelativePositionModel : AnnotationShapePositionModel {
		protected new RelativePosition ShapePosition { get { return (RelativePosition)base.ShapePosition; } }
		[PropertyForOptions,
		Category("General")]
		public double ConnectorLength {
			get { return ShapePosition.ConnectorLength; }
			set { SetProperty("ConnectorLength", value); }
		}
		[PropertyForOptions,
		Category("General")]
		public double Angle {
			get { return ShapePosition.Angle; }
			set { SetProperty("Angle", value); }
		}
		public RelativePositionModel(RelativePosition shapePosition, CommandManager commandManager)
			: base(shapePosition, commandManager) {
		}
	}
	public abstract class AxisCoordinateModel : DesignerChartElementModelBase {
		readonly AxisCoordinate axisCoordinate;
		protected AxisCoordinate AxisCoordinate { get { return axisCoordinate; } }
		protected internal override ChartElement ChartElement { get { return axisCoordinate; } }
		[
		TypeConverter(typeof(AxisValueTypeConverter)),
		PropertyForOptions,
		Category("General")]
		public object AxisValue {
			get { return AxisCoordinate.AxisValue; }
			set { SetProperty("AxisValue", value); }
		}
		public AxisCoordinateModel(AxisCoordinate axisCoordinate, CommandManager commandManager)
			: base(commandManager) {
			this.axisCoordinate = axisCoordinate;
		}
	}
	[ModelOf(typeof(AxisXCoordinate))]
	public class AxisXCoordinateModel : AxisCoordinateModel {
		Axis2DModel axisModel;
		protected new AxisXCoordinate AxisCoordinate { get { return (AxisXCoordinate)base.AxisCoordinate; } }
		[
		Editor("DevExpress.XtraCharts.Designer.Native.AxisXModelUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		UseAsSimpleProperty,
		PropertyForOptions,
		Category("General")]
		public Axis2DModel Axis {
			get { return axisModel; }
			set { SetProperty("Axis", value); }
		}
		public AxisXCoordinateModel(AxisXCoordinate axisCoordinate, CommandManager commandManager)
			: base(axisCoordinate, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (axisModel != null)
				Children.Add(axisModel);
			base.AddChildren();
		}
		public override void Update() {
			this.axisModel = (Axis2DModel)ModelHelper.CreateAxisModelInstance(AxisCoordinate.Axis, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(AxisYCoordinate))]
	public class AxisYCoordinateModel : AxisCoordinateModel {
		Axis2DModel axisModel;
		protected new AxisYCoordinate AxisCoordinate { get { return (AxisYCoordinate)base.AxisCoordinate; } }
		[
		Editor("DevExpress.XtraCharts.Designer.Native.AxisYModelUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		UseAsSimpleProperty,
		PropertyForOptions,
		Category("General")]
		public Axis2DModel Axis {
			get { return axisModel; }
			set { SetProperty("Axis", value); }
		}
		public AxisYCoordinateModel(AxisYCoordinate axisCoordinate, CommandManager commandManager)
			: base(axisCoordinate, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if (axisModel != null)
				Children.Add(axisModel);
			base.AddChildren();
		}
		public override void Update() {
			this.axisModel = (Axis2DModel)ModelHelper.CreateAxisModelInstance(AxisCoordinate.Axis, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
}
